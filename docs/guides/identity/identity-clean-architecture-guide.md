# ASP.NET Core Identity — Clean Architecture Rehberi

Bu belge, ASP.NET Core Identity'nin **Onion / Clean Architecture** içinde **doğru şekilde** nasıl konumlandırılması gerektiğini açıklar. Literatür (Jason Taylor, ardalis) ve topluluk tartışmalarına dayalı olarak hazırlanmıştır.

---

## 1. Temel Sorun: Neden Identity Katmana Özgü Bir Meseledir?

`UserManager<T>`, `SignInManager<T>`, `RoleManager<T>` ve `ApplicationUser : IdentityUser` gibi tipler **ASP.NET Core Identity** çerçevesine ait altyapı detaylarıdır. Bunları doğrudan Application katmanında (handler'larda) kullanmak, katman bağımlılık kuralını ihlal eder:

```
Domain ← Application ← Infrastructure ← Presentation
```

> **Kural:** Application katmanı yalnızca kendi tanımladığı soyutlamalara (interface) bağımlı olabilir;
> hiçbir zaman Infrastructure'a ait somut sınıflara (`UserManager`, `IdentityUser`, `ApplicationUser`) bağımlı olamaz.

### Yanlış kullanım (ihlal)
```csharp
// Application/Features/Auth/Commands/RegisterUser/RegisterUserCommandHandler.cs
public class RegisterUserCommandHandler(UserManager<ApplicationUser> userManager) // ❌ Infrastructure bağımlılığı
    : IRequestHandler<RegisterUserCommand, RegisterUserResponse>
{ ... }
```

### Doğru kullanım
```csharp
// Application/Features/Auth/Commands/RegisterUser/RegisterUserCommandHandler.cs
public class RegisterUserCommandHandler(IIdentityService identityService) // ✅ Soyutlamaya bağımlı
    : IRequestHandler<RegisterUserCommand, RegisterUserResponse>
{ ... }
```

---

## 2. Her Katmanın Rolü

### 2.1 Domain Katmanı (`Dev4All.Domain`)

Domain, business logic'in çekirdeğidir. Dışa bağımlılığı yoktur.

**Identity ile ilgili Domain sorumlulukları:**
- `UserRole` enum'u (iş kuralı olduğu için burada durur):
  ```csharp
  public enum UserRole { Customer = 0, Developer = 1, Admin = 2 }
  ```
- Rol sabitleri (string tabanlı kullanım gerekiyorsa):
  ```csharp
  public static class Roles
  {
      public const string Customer   = nameof(Customer);
      public const string Developer  = nameof(Developer);
      public const string Admin      = nameof(Admin);
  }
  ```

> **Not:** `IdentityRole` ve `ApplicationUser` **Domain'e ait değildir**;
> bunlar altyapı tipidir ve Infrastructure katmanında kalır.

---

### 2.2 Application Katmanı (`Dev4All.Application`)

Application katmanı, **ne yapılacağını** tanımlar; **nasıl yapılacağını** bilmez.

**Identity ile ilgili Application sorumlulukları:**
1. `IIdentityService` interface'ini tanımlar — Authentication/Authorization işlemlerinin sözleşmesi.
2. `IJwtService` interface'ini tanımlar — Token üretimi (zaten mevcut).
3. `ICurrentUser` interface'ini tanımlar — Aktif kullanıcı bilgisi (zaten mevcut).
4. Handler'lar bu interface'leri inject ederek kullanır.

#### `IIdentityService` Tasarımı

```csharp
// Application/Abstractions/Auth/IIdentityService.cs
namespace Dev4All.Application.Abstractions.Auth;

public interface IIdentityService
{
    /// <summary>Yeni kullanıcı oluşturur ve hashed password kaydeder.</summary>
    Task<(bool Succeeded, string UserId, IEnumerable<string> Errors)> CreateUserAsync(
        string name, string email, string password, string role, CancellationToken ct = default);

    /// <summary>E-posta + şifre ile kullanıcıyı doğrular; başarılıysa userId döner.</summary>
    Task<(bool Succeeded, string UserId, string Email, string Role)> AuthenticateAsync(
        string email, string password, CancellationToken ct = default);

    /// <summary>Kullanıcının belirli bir rolde olup olmadığını kontrol eder.</summary>
    Task<bool> IsInRoleAsync(string userId, string role, CancellationToken ct = default);

    /// <summary>UserId ile kullanıcı adını döner.</summary>
    Task<string?> GetUserNameAsync(string userId, CancellationToken ct = default);
}
```

> **Tasarım notu:** Metot imzaları **primitive türler** (string, bool) kullanır.
> `ApplicationUser`, `IdentityResult` gibi Infrastructure'a ait tipler imzalarda **asla görünmez**.
> Bu sayede Application katmanı derleyici düzeyinde Infrastructure'dan bağımsız kalır.

---

### 2.3 Infrastructure Katmanı (`Dev4All.Infrastructure`)

Infrastructure, **nasıl yapılacağını** bilir; framework detaylarını içerir.

**Identity ile ilgili Infrastructure sorumlulukları:**
1. `IIdentityService`'i `IdentityService` ile implemente eder.
2. `UserManager<ApplicationUser>`, `SignInManager<ApplicationUser>`, `RoleManager<IdentityRole>` burada kullanılır.
3. `IJwtService` → `JwtService` (zaten mevcut).
4. `ICurrentUser` → `CurrentUser` (zaten mevcut).

#### `IdentityService` Yapısı

```csharp
// Infrastructure/Auth/IdentityService.cs
using Dev4All.Application.Abstractions.Auth;
using Dev4All.Persistence.Identity;
using Microsoft.AspNetCore.Identity;

namespace Dev4All.Infrastructure.Auth;

public sealed class IdentityService(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager) : IIdentityService
{
    public async Task<(bool Succeeded, string UserId, IEnumerable<string> Errors)> CreateUserAsync(
        string name, string email, string password, string role, CancellationToken ct = default)
    {
        var user = new ApplicationUser { UserName = email, Email = email, Name = name };
        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
            return (false, string.Empty, result.Errors.Select(e => e.Description));

        await userManager.AddToRoleAsync(user, role);
        return (true, user.Id, []);
    }

    public async Task<(bool Succeeded, string UserId, string Email, string Role)> AuthenticateAsync(
        string email, string password, CancellationToken ct = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null) return (false, string.Empty, string.Empty, string.Empty);

        var valid = await userManager.CheckPasswordAsync(user, password);
        if (!valid) return (false, string.Empty, string.Empty, string.Empty);

        var roles = await userManager.GetRolesAsync(user);
        return (true, user.Id, user.Email!, roles.FirstOrDefault() ?? string.Empty);
    }

    public async Task<bool> IsInRoleAsync(string userId, string role, CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId);
        return user is not null && await userManager.IsInRoleAsync(user, role);
    }

    public async Task<string?> GetUserNameAsync(string userId, CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId);
        return user?.UserName;
    }
}
```

> **Not:** `ApplicationUser` tipi, `Dev4All.Persistence` projesindedir.
> `Dev4All.Infrastructure`, `Dev4All.Persistence`'a proje referansı vermelidir (veya `ApplicationUser` Infrastructure'a taşınmalıdır).
> Bu projede `ApplicationUser` **Persistence'da** tanımlıdır — bu da yaygın bir yaklaşımdır.

---

### 2.4 Persistence Katmanı (`Dev4All.Persistence`)

Persistence, veritabanı ile ilgili tüm altyapıyı barındırır.

**Identity ile ilgili Persistence sorumlulukları:**
- `ApplicationUser : IdentityUser` burada tanımlanır (zaten mevcut).
- `Dev4AllDbContext : IdentityDbContext<ApplicationUser>` burada tanımlanır (zaten mevcut).
- `AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<Dev4AllDbContext>()` → `Program.cs`'te kurulur.

---

### 2.5 Presentation Katmanı (`Dev4All.WebAPI`)

- `Program.cs`'te `AddIdentity(...)` + `AddEntityFrameworkStores<Dev4AllDbContext>()` çağrısı yapılır.
- Controller'lar yalnızca MediatR `ISender` üzerinden handler'lara delegate eder; Identity sınıflarına doğrudan bağımlı olmazlar.
- `[Authorize(Roles = "Developer")]` gibi attribute'lar bu katmanda kullanılabilir.

---

## 3. Katman Diyagramı

```
┌─────────────────────────────────────────────────────────┐
│  Domain                                                   │
│  UserRole enum, Roles constants                           │
│  (Identity framework bağımlılığı YOK)                     │
└────────────────────────┬────────────────────────────────┘
                         │ ← bağımlı
┌────────────────────────▼────────────────────────────────┐
│  Application                                              │
│  IIdentityService   ← interface sözleşmesi               │
│  IJwtService        ← interface sözleşmesi               │
│  ICurrentUser       ← interface sözleşmesi               │
│  Handlers → IIdentityService inject eder                  │
│  (Identity somut sınıfları ASLA burada değil)             │
└─────┬──────────────────────┬───────────────────────────┘
      │ ← bağımlı            │ ← bağımlı
┌─────▼──────────┐  ┌────────▼──────────────────────────┐
│  Infrastructure │  │  Persistence                       │
│  IdentityService│  │  ApplicationUser : IdentityUser    │
│  (UserManager   │  │  Dev4AllDbContext                  │
│   RoleManager)  │  │  (IdentityDbContext<AppUser>)      │
│  JwtService     │  │                                    │
│  CurrentUser    │  │                                    │
└─────┬───────────┘  └────────┬──────────────────────────┘
      │ ← bağımlı             │ ← bağımlı
┌─────▼───────────────────────▼──────────────────────────┐
│  WebAPI (Presentation)                                   │
│  Program.cs → AddIdentity, DI wire-up                    │
│  Controllers → ISender ile MediatR'a delegate            │
└─────────────────────────────────────────────────────────┘
```

---

## 4. Handler'larda Doğru Kullanım

### RegisterUser Handler

```csharp
// Application/Features/Auth/Commands/RegisterUser/RegisterUserCommandHandler.cs
using Dev4All.Application.Abstractions.Auth;
using Dev4All.Domain.Enums;
using MediatR;

public sealed class RegisterUserCommandHandler(
    IIdentityService identityService) : IRequestHandler<RegisterUserCommand, RegisterUserResponse>
{
    public async Task<RegisterUserResponse> Handle(
        RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var (succeeded, userId, errors) = await identityService.CreateUserAsync(
            request.Name,
            request.Email,
            request.Password,
            request.Role.ToString(),
            cancellationToken);

        if (!succeeded)
            throw new BusinessRuleViolationException(string.Join(", ", errors));

        return new RegisterUserResponse(Guid.Parse(userId), request.Email, request.Name);
    }
}
```

### LoginUser Handler

```csharp
// Application/Features/Auth/Commands/LoginUser/LoginUserCommandHandler.cs
using Dev4All.Application.Abstractions.Auth;
using MediatR;

public sealed class LoginUserCommandHandler(
    IIdentityService identityService,
    IJwtService jwtService) : IRequestHandler<LoginUserCommand, LoginUserResponse>
{
    public async Task<LoginUserResponse> Handle(
        LoginUserCommand request, CancellationToken cancellationToken)
    {
        var (succeeded, userId, email, role) = await identityService.AuthenticateAsync(
            request.Email, request.Password, cancellationToken);

        if (!succeeded)
            throw new UnauthorizedDomainException("Geçersiz e-posta veya şifre.");

        var token = jwtService.GenerateToken(userId, email, role);
        return new LoginUserResponse(token, DateTime.UtcNow.AddMinutes(120), email, role);
    }
}
```

### GetCurrentUser Handler

```csharp
// Application/Features/Auth/Queries/GetCurrentUser/GetCurrentUserQueryHandler.cs
using Dev4All.Application.Abstractions.Auth;
using MediatR;

public sealed class GetCurrentUserQueryHandler(
    ICurrentUser currentUser) : IRequestHandler<GetCurrentUserQuery, GetCurrentUserResponse>
{
    public Task<GetCurrentUserResponse> Handle(
        GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated)
            throw new UnauthorizedDomainException("Kullanıcı doğrulaması gereklidir.");

        return Task.FromResult(
            new GetCurrentUserResponse(currentUser.UserId, currentUser.Email, currentUser.Role));
    }
}
```

---

## 5. DI Kaydı

### `InfrastructureServiceRegistration.cs`

```csharp
services.AddScoped<IIdentityService, IdentityService>();
services.AddScoped<IJwtService, JwtService>();
services.AddScoped<ICurrentUser, CurrentUser>();
```

### `Program.cs` — Infrastructure `IdentityService`'in `ApplicationUser` kullananı için

`IdentityService`, `UserManager<ApplicationUser>` alır. `AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<Dev4AllDbContext>()` çağrısı zaten `UserManager`'ı DI'a kaydeder; ek bir kayıt gerekmez.

---

## 6. Sık Yapılan Hatalar

| Hata | Neden Yanlış | Doğrusu |
|---|---|---|
| Handler'da `UserManager<ApplicationUser>` inject etmek | Application → Infrastructure bağımlılığı yaratır | `IIdentityService` inject et |
| `ApplicationUser`'ı Application projesinde tanımlamak | IdentityUser framework bağımlılığı taşır | Persistence veya Infrastructure'da tut |
| `IdentityResult` döndüren interface metodları | Infrastructure tipini Application'a sızdırır | Primitive `(bool, string, IEnumerable<string>)` tuple kullan |
| `RoleManager`'ı handler'da kullanmak | Rol yönetimi altyapı detayıdır | `IIdentityService.IsInRoleAsync` arkasında sakla |

---

## 7. Referans Projeler

| Proje | Yaklaşım |
|---|---|
| [jasontaylordev/CleanArchitecture](https://github.com/jasontaylordev/CleanArchitecture) | `IIdentityService` Application'da, `IdentityService` Infrastructure'da |
| [ardalis/CleanArchitecture](https://github.com/ardalis/CleanArchitecture) | Domain'de `User` aggregate, Identity Infrastructure'da |
| [feriel214/ASP.NET-Core-CleanArchitecture-CQRS-Identity](https://github.com/feriel214/asp.net-core-cleanarchitecture-cqrs-identity) | CQRS + MediatR + Identity ayrımı |

---

## 8. Bu Projeye Özgü Uygulama Notu

`Dev4All` projesinde güncel durum:
- `ApplicationUser` → `Dev4All.Persistence/Identity/ApplicationUser.cs` ✅ (doğru yer)
- `UserRole` enum → `Dev4All.Domain/Enums/UserRole.cs` ✅ (doğru yer)
- `IJwtService` → `Dev4All.Application/Abstractions/Auth/IJwtService.cs` ✅
- `ICurrentUser` → `Dev4All.Application/Abstractions/Auth/ICurrentUser.cs` ✅
- `IIdentityService` → `Dev4All.Application/Abstractions/Auth/IIdentityService.cs` ✅
- `IdentityService` → `Dev4All.Infrastructure/Auth/IdentityService.cs` ✅
- Auth handler'ları (`RegisterUser`, `LoginUser`, `GetCurrentUser`) artık `NotImplementedException` yerine ilgili abstractions üzerinden çalışıyor ✅

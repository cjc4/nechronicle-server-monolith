module NechronicleOpenAPIServer.AppUserRepository

open System

let listAppUser queryParameters =
    let mockBob =
        {
            ID = AppUserUUID(Guid.Parse("01982ea1-6de2-7195-8100-2c967a64288b"))
            Username = "burgermaster"
            Email = Email "bob@example.com"
            FirstName = Some("Bob")
            LastName = Some("Belcher")
        } : AppUser
    let mockGandalf =
        {
            ID = AppUserUUID(Guid.Parse("01982ea2-f46d-7dd4-bb53-1ba39d458052"))
            Username = "wise1"
            Email = Email "gandalf@example.com"
            FirstName = Some("Gandalf")
            LastName = Some("Greyhame")
        } : AppUser
    let mockList = [mockBob; mockGandalf]
    Ok mockList

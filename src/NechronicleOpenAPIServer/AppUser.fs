module NechronicleOpenAPIServer.AppUser

let create id username email firstName lastName =
        {
            ID = id
            Username = username
            Email = email
            FirstName = firstName
            LastName = lastName
        }

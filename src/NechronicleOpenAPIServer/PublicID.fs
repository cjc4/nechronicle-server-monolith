module NechronicleOpenAPIServer.PublicID

// Note to self: this file should no longer be used, left here for future reference

// Third party dependencies
open FsRandom

let generate state =
    let generator = random {
        // Configure generator to generate numbers from zero to one.
        let! x = ``[0, 1)``
        return x
    }
    let randomValue = Random.get generator state
    let idNum = int (randomValue * 10000.0)
    PublicID(idNum.ToString())

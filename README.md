# RMIT Games and AI SolGame
## How to run
Please download the project, and open it via your unity hub.

Download the following asset, and insert it into the scene.
https://assetstore.unity.com/packages/3d/props/breakable-asteroids-167825
Note:Since ”Assets\Models\BreakableAsteroids\Asteroids\zasteroids.dae” model are larger than 100M and cannot be uploaded to Github.

Enjoy :)

## Branching Strategy

To add a feature, please take a branch from `main`, please title it `feature/your_feat_name`

When done please make a PR back to `main`

If you can please try to use gitflow for your commit messages e.g.
- `feat`: (new feature for the user, not a new feature for build script)
- `fix`: (bug fix for the user, not a fix to a build script)
- `docs`: (changes to the documentation)
- `style`: (formatting, missing semi colons, etc; no production code change)
- `refactor`: (refactoring production code, eg. renaming a variable)
- `test`: (adding missing tests, refactoring tests; no production code change)
- `chore`: (updating grunt tasks etc; no production code change)



# Game Behaviour:

## steering behaviours [boids]

follow-the-leader

wall following

a seek behaviour that avoids overshooting (arrive)

separation, cohesion and alignment


## pathfinding

correctly calculates the shortest path to the target 

smoothed 

Pathfinding is integrated with steering（via forces/acceleration）（via forces/acceleration）

periodically updating the path

world are associated with a higher path cost than others (e.g., swamp versus dry land).

## FSMs

At least two types of character use FSMs

## Game build

Graphics, animation and sound

collectable items / weapons

multiple types of prey or predator

procedurally generated levels

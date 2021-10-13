# CS4500 - The Rectangle Problem
This project is based on this [Numberphile](https://www.youtube.com/watch?v=VZ25tZ9z6uI) video.

### Git Usage
| Git Terms | Definition |
| ----- | ---------  |
| Repository (repo) | Kind of like a project folder, holds every file and a history of changes. |
| Remote | A repository but on a server (in our case Github.com). |
| Branch | Stores a subfolder that used to experiment with code without changing that code in other folders. |
| Commit | Snapshot of a program |
| Clone | A copy of a repository. |

| Basic Git Commands | Function |
| ----- | ---------  |
| Checkout | Allows you to select different branches and load that code locally without interrupts |
| Fetch | Downloads changes from the remote repository without overriding your changes. Use to stay updated with remote repo. |
| Pull | Downloads **and** merges remote repo changes into current local code. It is recommended to `stash` if you want to keep your local changes.
| Stash | Saves a record of all local files. Used to safely keep changes you make before merging/pulling from the repo. |

#### Recommended Workflow
1. Check Trello board for tasks to work on
2. `fetch origin` to sync latest updates to code
    - **Note**: You may have to set the link to the repository if using the terminal to get updates
        - `git remote add origin git@github.com:br36b/CS4500-The-Rectangle-Problem.git`
        - Github Desktop: Use the `File > Clone repository...` option
    - `git fetch origin`
    - On Github Desktop: `Fetch origin` Button Top-Right
3. If you want to download **and merge** new changes from the repo
      - **Note**: *If you made changes to code that was altered by someone else, you should* `git stash` *to avoid losing your changes.*
          - ex. `git stash` -> `git pull` -> `git stash pop`
          - This will allow you to use the pull command and pick code to keep in any code conflicts
      - `git pull`
      - On Github Desktop: `Repository -> Pull`
4. Can alternatively `checkout` to download different code versions and view it locally without overriding local changes.
    - `git checkout <branchname>`
    - Github Desktop: `Current Branch` menu, however only works for branches at the moment.
5. After making changes to keep
    - Use `add` to select changes to add to a commit
      - `git add .` ('dot' selects all files in the directory, can also individually type every file name)
          - `git status` can be used to see files added
      - Github Desktop: `Changes` Panel, checkmark indicates a file will be included in the next commit
    - `git commit -m "Some information about the changes you made"`
    - On Github Desktop: `Summary & Description` bottom-left under changes panel
6. Pushing changes to group repo
    - Up to this point, any changes/commits are only local
        - To sync these changes with the Github repo, you must **push** those changes
    - For now, we will only use the main branch
        - `git push main`
        - Github Desktop: `Repository > Push`

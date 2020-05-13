# Contribution Guidelines

Mixed Reality Toolkit is under the [MIT license](https://github.com/nunit/nunit/blob/master/LICENSE.txt). By contributing to the Mixed Reality Toolkit, you assert that:

* The contribution is your own original work.
* The contribution adheres to the [Coding Guidelines](articles/A01-CodingGuidelines.md)
* You have the right to assign the copyright for the work (it is not owned by your employer, or
  you have been given copyright assignment in writing).

## How to contribute

### Prerequisites

* A GitHub Account
* Familiarization with projects with Git Source conrol versioning. Atlassian has a wonderful guide for [getting started with Git](https://www.atlassian.com/git).
* Install Git on your local machine and have git assigned as an envionment variable.
* Install a Git client like [Fork](https://git-fork.com/) or [GitHub for Desktop](https://desktop.github.com/) for staging and committing code to source control
* Follow any [Getting Started Guidelines](articles\00-GettingStarted.md#prerequisites) for setting up your development envrionment not covered here.

### Steps

1. Fork the repository you'd like to open a pull request for.
2. Sync any changes from the source repository to your fork.
3. Create a new branch based on the last source development commit.
4. Make the changes you'd like to contribute.
5. Stage and commit your changes with thoughtful messages detailing the work.
6. Push your local changes to your fork's remote server.
7. Navigate to the repository's source repository on GitHub.
    > **Note:** by now a prompt to open a new pull request should be availible on the respository's main landing page.
8. Open a pull request detailing the changes and fill out the Pull Request Template.
9. Typically Code Reviews are performed in around 24-48 hours.
10. Iterate on any feedback from reviews.
    > **Note:** Typically you can push the changes directly to the branch you've opened the pull request for.
11. Once the pull request is accepted and the build vaidation passes, changes are then squashed and merged into the target branch, and the process is repeated.

### Branching Strategies

The master, development, and any feature branches are all protected by branch policies. Each branch must be up to date and pass test and build validation before it can be merged.

* All merges to the feature and development branches are squashed and merged into a single commit to keep the history clean and focused on specific pull request changes.
* All merges from the master and development branches are just traditionally merged together to ensure they stay in sync and share the same histories.

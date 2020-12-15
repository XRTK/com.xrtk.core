# Contribution Guidelines

Mixed Reality Toolkit is under the [MIT license](https://github.com/nunit/nunit/blob/master/LICENSE.txt). By contributing to the Mixed Reality Toolkit, you assert that:

* The contribution is your own original work.
* The contribution adheres to the [Coding Guidelines](articles/appendices/A01-CodingGuidelines.md)
* You have the right to assign the copyright for the work (it is not owned by your employer, or
  you have been given copyright assignment in writing).

## How to contribute

### Prerequisites

* A GitHub Account
* Familiarization with projects with Git Source control versioning. Atlassian has a wonderful guide for [getting started with Git](https://www.atlassian.com/git).
* Install Git on your local machine and have git assigned as an environment variable.
* Install a Git client like [Fork](https://git-fork.com/) or [GitHub for Desktop](https://desktop.github.com/) for staging and committing code to source control
* Follow any [Getting Started Guidelines](articles/00-GettingStarted.md#prerequisites) for setting up your development environment not covered here.

### Steps

1. Fork the repository you'd like to open a pull request for.
2. Clone or sync any changes from the source repository to your local disk.
    > **Note:** When initially cloning the repository be sure to recursively check out all submodules!
3. Create a new branch based on the last source development commit.
4. Make the changes you'd like to contribute.
5. Stage and commit your changes with thoughtful messages detailing the work.
6. Push your local changes to your fork's remote server.
7. Navigate to the repository's source repository on GitHub.
    > **Note:** by now a prompt to open a new pull request should be available on the repository's main landing page.
8. Open a pull request detailing the changes and fill out the Pull Request Template.
9. Typically Code Reviews are performed in around 24-48 hours.
10. Iterate on any feedback from reviews.
    > **Note:** Typically you can push the changes directly to the branch you've opened the pull request for.
11. Once the pull request is accepted and the build validation passes, changes are then squashed and merged into the target branch, and the process is repeated.

### Branching Strategies

The master, development, and any feature branches are all protected by branch policies. Each branch must be up to date and pass test and build validation before it can be merged.

* All merges to the feature and development branches are squashed and merged into a single commit to keep the history clean and focused on specific pull request changes.
* All merges from the master and development branches are just traditionally merged together to ensure they stay in sync and share the same histories.

---

### [**Raise an Information Request**](https://github.com/XRTK/XRTK-Core/issues/new?assignees=&labels=question&template=request_for_information.md&title=)

If there is anything not mentioned in this document or you simply want to know more, raise an [RFI (Request for Information) request here](https://github.com/XRTK/XRTK-Core/issues/new?assignees=&labels=question&template=request_for_information.md&title=).

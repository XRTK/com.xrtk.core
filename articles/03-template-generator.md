# Platform Template Generation

New platforms are always welcomed with open arms! If you'd like to add your platform's support, this article will guide you through the process.

## What you'll need

The XRTK uses a [template repository](https://github.com/XRTK/PlatformTemplate) to quickly add new platform support. You'll need to either clone this or, if you're a contributor to the XRTK team directly, you can create a new repository using this as a template.

## Getting Started

1. Create a new repository on Github using the template repository: ![Create New Repository](/images/TemplateGeneration/NewRepoButton.png)
![New Repository Settings](/images/TemplateGeneration/NewRepoSettings.png)
2. Clone the new Repository ![Clone New Repository](/images/TemplateGeneration/CloneRepo.png)![Clone Local Repository](/images/TemplateGeneration/CloneLocalRepo.png)
3. Navigate to the root of the new repository and run `InitializeTempalte.ps1` using the command line ![Root Project Folder](/images/TemplateGeneration/ProjectRootExplorer.png)
4. Type the name of your new project and press `Enter` ![Run InitiazeTempalte](/images/TemplateGeneration/RunPowershell.png)
5. Stage, Commit, and Push the updated files directly to the new master branch origin ![Commit Files](/images/TemplateGeneration/CommitToMaster.png)
6. Create the development branch and push it to the origin ![Development Branch](/images/TemplateGeneration/DevelopmentBranch.png)
    > This branch should be at the same checkout as your master branch
    ![Development Checkout](/images/TemplateGeneration/DevelopmentCommit.png)
7. Open the new project in Unity and verify the project is setup correctly ![](/images/TemplateGeneration/LaunchUnity.png)
8. Create a new branch and push the updated changes to the meta files and generated links ![](/images/TemplateGeneration/BranchMetaFiles.png) ![Meta File Branch Commit](/images/TemplateGeneration/CommitToMetaFiles.png)
9. Back in GitHub, navigate to the project's settings
    - Update the following features:![Disabled Features](/images/TemplateGeneration/DisableFeatures.png)
    ![Merge Button](/images/TemplateGeneration/MergeButtonFeatures.png)
10. Update the default branch to `development` ![Default Branch](/images/TemplateGeneration/DefaultBranch.png)
11. Add branch protection rules to `master`, `development`, and `features/*` ![Branch Protection Rules](/images/TemplateGeneration/BranchProtections-01.png)
    > **Note:** The required status checks will not show until the pipeline has ran at least once. You'll need to come back and enable this once the pipeline is setup and the initial PR is opened.

    > **Note:** The `features/*` rule will have have an additional setting `Allow deletions` so that they can be removed after the PR is compelted.
12. Create a new Pipeline in Azure Devops ![Create new pipeline](/images/TemplateGeneration/NewPipeline-01.png)
    > **Note:** This step requires an admin to the XRTK to complete. If you're using the project template in your own devops envirionment then the steps are the same.
13. Select the GitHub repository in the pipeline wizard ![](/images/TemplateGeneration/NewPipeline-02.png) ![](/images/TemplateGeneration/NewPipeline-03.png)
14. Save the pipeline ![](/images/TemplateGeneration/NewPipeline-04.png)
15. Edit the pipeline and update the name and triggers ![](/images/TemplateGeneration/NewPipeline-05.png) ![](/images/TemplateGeneration/NewPipeline-06.png) ![](/images/TemplateGeneration/NewPipeline-07.png)
16. Save the pipeline, but **DO NOT RUN!** ![](/images/TemplateGeneration/NewPipeline-08.png)
17. Open a pull request in your new GitHub repository for the meta file changes ![](/images/TemplateGeneration/PullRequest-01.png) ![](/images/TemplateGeneration/PullRequest-02.png)
    > **Note:** You should see status checks start once Pull Request is opened

    ![](/images/TemplateGeneration/PullRequest-03.png)

    > **Note:** Once the status checks have ran at least once, you can go back to your branch protection rules and add the check

    ![](/images/TemplateGeneration/BranchProtections-02.png)

---

### [**Raise an Information Request**](https://github.com/XRTK/XRTK-Core/issues/new?assignees=&labels=question&template=request_for_information.md&title=)

If there is anything not mentioned in this document or you simply want to know more, raise an [RFI (Request for Information) request here](https://github.com/XRTK/XRTK-Core/issues/new?assignees=&labels=question&template=request_for_information.md&title=).

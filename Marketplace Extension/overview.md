# Test Case Association Manager
Azure DevOps extension for managing the associations between Test Methods (code) and Test Cases (Azure DevOps). It uses the [Azure DevOps Services REST API] to achieve this. The Test Method loading happens through its assemblies. These assemblies will be detected by using minimatch patterns which are provided by the user.

# 1. Features
* **Associations**: associates Test Methods (code) with Test Cases (Azure DevOps) by ***name***.
* **Validation only**: if this option is selected the Association manager will only validate the associations without persisting the changes. It is advised to use this option in a pull request build definition. 
* **Fixing broken associations**: outdated associations will be fixed.
  Scenario: 
  1. Run Association manager: associates Test Method 'Test29' with Test Case 'Test29'.
  2. User removes Test Method 'Test29' and creates Test Method 'Test30', renames Test Case 'Test29' to 'Test30'. (Test Case 'Test30' is now still associated to the missing Test Method 'Test29'). 
  3. Run Association manager: fixes the association by removing/overwriting the outdated association.
  4. Test Case 'Test30' is now associated with Test Method 'Test30'.
* **Pipeline reporting & flow handling**: outputs every status to the console in real-time. **Any error** (reading/loading/associating) will cause the *pipeline job* to fail.
* **Verbose logging**: once enabled, the manager will also output the successful matchings next to the warnings/errors/fixes.
* **Missing Test cases (warning)**: detects missing Test Cases.
* **Duplicate Test Methods (warning)**: detects duplicate Test Methods.
* **Duplicate Test Cases (warning)**: detects duplicate Test Cases.
* **Deprecated Test Methods (warning)**: detects deprecated Test Methods (outdated).
* **Association overview**: outputs an overview of the association process at the end.

# 2. Prerequisite
* **Personal Access Token** 
The Azure DevOps Services REST API requires user authentication. For security reasons the authentication happens through a Personal Access Token only. For the creation of this token, please see chapter *3.1 Personal Access Token*.

# 3. Installation
## 3.1 Personal Access Token
To create a Personal Access Token please read chapter 'Create personal access tokens to authenticate access' of [this guide]. When you're creating the PAT it is important to select **'All accessible organizations'** under **Organization** and set the correct **Custom defined** scopes:
* **Work items**: Read & write; 
* **Test Management**: Read & write.
![PAT settings](https://image.frl/i/s3blya3zo18bvzyh.png)

# 4. Setup
## 4.1 Variables
| Name | Required | Explanation | Example |
| :--- | :---: | :--- | :--- |
| SourceFolder | **X**  | The root directory to search in. | ``` $(System.DefaultWorkingDirectory) ``` |
| MinimatchPatterns | **X** | The [minimatch patterns] to search for within the directory, separated by a semicolon. | ``` **\$(BuildConfiguration)\**\*Test.Integration*.dll;**\$(BuildConfiguration)\**\*Test.Unit*.dll;!**\obj\** ``` |
| CollectionUri | **X** | The Azure DevOps collection Uri used for accessing the project Test Cases. | ``` https://testorganization.visualstudio.com/DefaultCollection ``` |
| PersonalAccesstoken | **X** | The Personal Access Token used for accessing the Azure DevOps project. | ``` d55d2f8e-f210-45a0-b09e-be55aeb835ea ``` |
| ProjectName | **X** | The Project name containing the Test Plan. | ``` HelloProject ``` |
| TestPlanName | **X** | The name of the Test Plan containing the Test Cases. | ``` System Test Plan ``` |
| TestType |  | Specifies the Test Type. You could leave this field *empty*. As far as we know it does nothing other than filling its field in a Test Case. |  |
| ValidateOnly |  | Indicates if you only want to validate the associations without persisting the changes. This makes it possible to insert this task within your pull request definition. |  |
| VerboseLogging |  | When Verbose logging is turned on it also outputs the successful matchings and the fixes next to the warnings/errors. |  |

## 4.2 Usage
There are two ways to create a pipeline definition: the old fashioned way or through YAML. Within this chapter the usage of this task will be explained for both methods.

**Caution!** The position of the task is very important. It should **always** be placed ***after*** the **Build Task**, but ***before*** the **Test Run Task**.

### 4.2.1 Non-YAML
![](https://image.frl/i/kkd7f6foylr083sz.png)

When you're setting up the **Run test task**, it is important to select **Test plan** under **Select tests using**. This makes it possible to link the outcome of the Test Run to the Test plan.
![](https://image.frl/i/voqc1wcd384ef810.png)

### 4.2.2 YAML
```
- task: QNH-Consulting-BV.AssociateToolTask.AssociateToolTask.AssociateTestMethodsWithTestCases@1
  displayName: 'Test Case Association Manager'
  inputs:
    sourceFolder: '$(System.DefaultWorkingDirectory)'

    minimatchPatterns: '**\$(BuildConfiguration)\**\*Test.Integration*.dll;**\$(BuildConfiguration)\**\*Test.Unit*.dll;!**\obj\**'

    collectionUri: 'https://testorganization.visualstudio.com/DefaultCollection'

    personalAccesstoken: 'd55d2f8e-f210-45a0-b09e-be55aeb835ea'

    projectName: 'HelloProject'

    testPlanName: 'System Test Plan'

    testType: ''

    validateOnly: true

    verboseLogging: true
```

When you're setting up the **Run Test task**, it is important to set the **testSelector**-property to  **'testPlan'**. This makes it possible to link the outcome of the Test Run to the Test plan.
```
- task: VSTest@2
  displayName: 'Run unit tests using test plan'
  inputs:
    testSelector: testPlan
    testPlan: 51
    testSuite: 52
    testConfiguration: 3
    searchFolder: '$(System.DefaultWorkingDirectory)\Test\Unit'
    vsTestVersion: toolsInstaller
    runInParallel: true
    codeCoverageEnabled: true
    platform: '$(BuildPlatform)'
    configuration: '$(BuildConfiguration)'
```

# 5. Contribute, Feedback, Issues
If you want to contribute, have some feedback, or report any issues, don't hesitate sending us an [email]. 

# 6. Contributers
We would like to thank [Flaticon] for their beautiful icons created by [Freepik] which are used in the creation of this extension logo ([CC BY 3.0]).


[//]: # (Reference links placement)
   [Azure DevOps Services REST API]: <https://docs.microsoft.com/en-us/rest/api/azure/devops/>
   [this guide]: <https://docs.microsoft.com/nl-nl/azure/devops/organizations/accounts/use-personal-access-tokens-to-authenticate?view=vsts>
   [minimatch patterns]: <https://docs.microsoft.com/en-us/azure/devops/pipelines/tasks/file-matching-patterns?view=vsts>
   [email]: <mailto:visualstudio@qnh.nl>
   [Freepik]: <https://www.freepik.com/>
   [Flaticon]: <https://www.flaticon.com>
   [CC BY 3.0]: <http://creativecommons.org/licenses/by/3.0/>
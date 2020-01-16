# Test Case Association Manager
Azure DevOps extension for managing the associations between Test Methods (code) and Test Cases (Azure DevOps). It uses the [Azure DevOps Services REST API] to achieve this. The Test Method loading happens through its assemblies. These assemblies will be detected by using minimatch patterns which are provided by the user.

# 0. Changelog
* **Version 2.0**
  - **.NET Core support**
  - Better task input options
  - Better and more output messages
* **Version 1.0**
  - First stable version.

# 1. Features
* **Association**: associates Test Methods (code) with Test Cases (Azure DevOps) by ***matching names***.
* **Validation only**: if this option is selected the Association manager will only validate the associations without persisting the changes. It is advised to use this option in a pull request build definition. 
* **Fixing broken associations**: outdated associations will be fixed.
  Scenario: 
  1. 1st run Association Manager: associates Test Method 'Test29' with Test Case 'Test29'.
  2. User removes Test Method 'Test29' and creates Test Method 'Test30', renames Test Case 'Test29' to 'Test30'. (Test Case 'Test30' is now still associated to the missing Test Method 'Test29'). 
  3. 2nd run Association Manager: fixes the association by removing/overwriting the outdated association.
  4. Test Case 'Test30' is now associated with Test Method 'Test30'.
* **Pipeline reporting & flow handling**: outputs every status to the console in real-time. **Any error** (reading/loading/associating) will cause the *pipeline job* to fail.
* **Verbose logging**: once enabled, the manager will also output the successful matchings next to the warnings/errors/fixes.
* **Missing Test Case detection**: detects missing Test Cases.
* **Duplicate Test Method detection**: detects duplicate Test Methods.
* **Duplicate Test Case detection**: detects duplicate Test Cases.
* **Deprecated Test Method detection**: detects deprecated (outdated) Test Methods.
* **Association overview**: outputs an overview of the association process at the end.

# 2. Prerequisite
* **Personal Access Token** 
The Azure DevOps Services REST API requires user authentication. For security reasons the authentication happens through a Personal Access Token only. For the creation of this token, please see chapter *3.1 Personal Access Token*.

# 3. Installation
## 3.1 Personal Access Token
To create a Personal Access Token please read chapter 'Create personal access tokens to authenticate access' of [this guide]. When you're creating the PAT it is important to select **'All accessible organizations'** under **Organization** and set the correct **Custom defined** scopes:
* **Work items**: Read & write; 
* **Test Management**: Read & write.
![PAT settings](https://associationex.blob.core.windows.net/documentation/BvJkre6.png)

# 4. Setup
## 4.1 Variables
### 4.1.1 Version 1
| Name | Required | Explanation | Example |
| :--- | :---: | :--- | :--- |
| SourceFolder | **X**  | The root directory to search in. | ``` $(System.DefaultWorkingDirectory) ``` |
| MinimatchPatterns | **X** | The [minimatch patterns] to search for within the directory, separated by a semicolon. | ``` **\$(BuildConfiguration)\**\*Test.Integration*.dll;**\$(BuildConfiguration)\**\*Test.Unit*.dll;!**\obj\** ``` |
| CollectionUri | **X** | The Azure DevOps collection Uri used for accessing the project Test Cases. | ``` https://testorganization.visualstudio.com/DefaultCollection ``` |
| PersonalAccesstoken | **X** | The Personal Access Token used for accessing the Azure DevOps project. | ``` d55d2f8e-f210-45a0-b09e-be55aeb835ea ``` |
| ProjectName | **X** | The Project name containing the Test Plan. | ``` HelloProject ``` |
| TestPlanName | **X** | The **name** of the Test Plan containing the Test Suites. | ``` System Test Plan ``` |
| TestPlanSuite | **X** | The **name** of the Test Suite containing the Test Cases. | ``` System Test Plan ``` |
| TestType |  | Specifies the Test Type. You could leave this field *empty*. As far as we know it does nothing other than filling its corresponding field within a Azure DevOps Test Case. | ``` unit ``` |
| ValidateOnly |  | Indicates if you only want to validate the associations without persisting the changes. This makes it possible to insert this task within your pull request definition. |  |
| VerboseLogging |  | When Verbose logging is turned on it also outputs the successful matchings and the fixes next to the warnings/errors. |  |

### 4.1.2 Version 2
| Name | Required | Explanation | Example |
| :--- | :---: | :--- | :--- |
| SourceFolder | **X**  | The root directory to search in. | ``` $(System.DefaultWorkingDirectory) ``` |
| MinimatchPatterns | **X** | The [minimatch patterns] to search for within the directory, separated by a semicolon. | ``` **\$(BuildConfiguration)\**\*Test.Integration*.dll;**\$(BuildConfiguration)\**\*Test.Unit*.dll;!**\obj\** ``` |
| PersonalAccesstoken | **X** | The Personal Access Token used for accessing the Azure DevOps project. | ``` d55d2f8e-f210-45a0-b09e-be55aeb835ea ``` |
| TestPlanId | **X** | The **id** of the Test Plan containing the Test Suites. | ``` 51 ``` |
| TestPlanSuiteId | **X** | The **id** of the Test Suite containing the Test Cases. | ``` 52 ``` |
| TestType |  | Specifies the Test Type. You could leave this field *empty*. As far as we know it does nothing other than filling its corresponding field within a Azure DevOps Test Case. | ``` integration ``` |
| ValidateOnly |  | Indicates if you only want to validate the associations without persisting the changes. This makes it possible to insert this task within your pull request definition. |  |
| VerboseLogging |  | When Verbose logging is turned on it also outputs the successful matchings and the fixes next to the warnings/errors. |  |

## 4.2 Usage
There are two ways to create a pipeline definition: the old fashioned way or through the new YAML definition. Within this chapter the usage of this task will be shown for both methods. This will be done through **examples**.

### 4.2.1 Non-YAML
#### 4.2.1.1 Version 1
![](https://associationex.blob.core.windows.net/documentation/50AcCSY.png)
**Caution!** The position of the task is very important. It should **always** be placed ***after*** the **Build Task**, but ***before*** the **Test Run Task**.

#### 4.2.1.2 Version 2
![](https://associationex.blob.core.windows.net/documentation/taskuiv2.PNG)
**Caution!** The position of the task is very important. It should **always** be placed ***after*** the **Build Task**, but ***before*** the **Test Run Task**.

#### 4.2.1.3 Run Test Task
When you're setting up the **Run test task**, it is important to select **Test plan** under **Select tests using**. This makes it possible to link the outcome of the Test Run to the Test plan.
![](https://associationex.blob.core.windows.net/documentation/testpl.png)

### 4.2.2 YAML
Although you can create your YAML definition with any preferred texteditor, it is highly advised to use the **Azure DevOps pipeline editor**. This editor comes with a task assistant which makes configuring a pipeline very easy by creating the task for you within your YAML definition.

If you can't find the assistant on your view, you should press the **Show assistant** button.
![](https://associationex.blob.core.windows.net/documentation/ShowAssistant.png)

When you select the extension, the following task configuration menu will show up. Some fields are filled in, some are not. You are free to change any value within this screen to your liking.
![](https://associationex.blob.core.windows.net/documentation/Assistant.png)

#### 4.2.2.1 Version 1
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
**Caution!** The position of the task is very important. It should **always** be placed ***after*** the **Build Task**, but ***before*** the **Test Run Task**.

#### 4.2.2.2 Version 2
```
- task: AssociateTestMethodsWithTestCases@1
  inputs:
    SourceFolder: '$(System.DefaultWorkingDirectory)'
    MinimatchPatterns: '**\$(BuildConfiguration)\**\*Test.Integration*.dll;**\$(BuildConfiguration)\**\*Test.Unit*.dll;!**\obj\**'
    PersonalAccesstoken: '$(PersonalAccessToken)'
    TestPlanId: '51'
    TestSuiteId: '52'
    TestType: 'unit'
    ValidateOnly: true
    VerboseLogging: true
```
**Caution!** The position of the task is very important. It should **always** be placed ***after*** the **Build Task**, but ***before*** the **Test Run Task**.

#### 4.2.2.3 Run Test Task
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
If you want to contribute, have some feedback, or report an issue, don't hesitate sending us an [email]. 

# 6. Github
Good news! This is an FOSS project. The source code can be found on [Github].


[//]: # (Reference links placement)
   [Azure DevOps Services REST API]: <https://docs.microsoft.com/en-us/rest/api/azure/devops/>
   [this guide]: <https://docs.microsoft.com/en-us/azure/devops/organizations/accounts/use-personal-access-tokens-to-authenticate?view=azure-devops>
   [minimatch patterns]: <https://docs.microsoft.com/en-us/azure/devops/pipelines/tasks/file-matching-patterns?view=azure-devops>
   [email]: <mailto:visualstudio@ilionx.com>
   [Github]: <https://github.com/ilionx/Test-Case-Association-Manager>
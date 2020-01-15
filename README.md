# Test Case Association Manager
##### Azure DevOps extension for managing the associations between Test Methods (code) and Test Cases (Azure DevOps).
The extension has been written in **.NET Core.** and therefore can be run locally through the command line. It supports **.NET and .NET Core applications**. For the production version please click on the following [link].

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

# 2. Setup
The extension has been written in **.NET Core.** and therefore can be run locally through the command line. The setup is very straightforward, just clone the repository, and then open the solution file (**.sln**).

# 3. Build & Publish Extension
1. Install the [tfx-cli] if you haven't yet.
2. Copy the **Marketplace Extension** folder to any directory outside of the repository. (Desktop will do).
3. Next, create the **tool** folder inside the **associatetooltask** folder. This specific folder can be found inside the **Marketplace Extension** folder.
4. Now build .NET Core project in **Release Mode**.
5. Copy **everything** from within directory **/bin/Release/netcoreappX.Y/** to the **tool** folder.
6. Change the **icon, overview.md, vss-extension.json, and task.json** files to your liking. These files can be found inside the **Marketplace Extension** folder.
7. Now open **CMD** and navigate to the root of the **Marketplace Extension** folder.
8. To create the extension just type '**tfx extension create**'. This command will generate the Microsoft Visual Studio Extension (.vsix) within the root of the folder.
9. You're all set! It's time to publish your extension.

# 4. Contribute, Feedback, Issues
If you want to contribute, have some feedback, or report an issue, don't hesitate contacting [us]!


[//]: # (Reference links placement)
   [Azure DevOps Services REST API]: <https://docs.microsoft.com/en-us/rest/api/azure/devops/>
   [us]: <mailto:visualstudio@ilionx.com>
   [tfx-cli]: <https://www.npmjs.com/package/tfx-cli>
   [link]: <https://marketplace.visualstudio.com/items?itemName=ilionx.AssociateToolTask>

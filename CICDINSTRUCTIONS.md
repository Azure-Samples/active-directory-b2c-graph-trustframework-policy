# Deploy custom policies from your Azure DevOps pipeline

This guide shows how to use Microsoft Graph apis for managing custom policies to deploy custom policies as part of your Azure DevOps pipeline. 

## Getting started

### Prerequisites

This sample requires the following:

* [Azure DevOps pipeline](https://azure.microsoft.com/en-us/services/devops/pipelines/)
* [Azure AD B2C tenant](https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-get-started)


### Create a web application in Azure AD
1. Sign in to the [Azure Portal](https://portal.azure.com/) using your Microsoft account.
1. Go to your Azure AD B2C tenant. you can do this by selecting book binding icon in top right corner portal.
1. Select Azure Active Directory blade in the portal. Form the left menu, select App registrations (Legacy). 
1. From the command bar, select + icon which says 'New application registration'.
1. Enter configuration as suggested below and select 'Create'
    1. Name :a name of your choice. Let's call it B2CDeployApp.
    1. Application type : Web app/API
    1. Sign-on URL : https://jwt.ms (you can choose any url of your choice).

1. After the app is created. Go to the app, select 'Settings' and then 'Required Permissions' from the menu.
1. Select 'Add'. And then 'Select an API' 
1. Select 'Microsoft Graph' from the list and then select Create.
1. Select 'Select permissions'. From **APPLICATION PERMISSIONS**, check 'Read and write your organization's trust framework policies'. 
1. Choose 'Select' at the bottom, and then 'Done'.
1. Select 'Grant permissions' to grant newly selected permissions consent to the app. 
1. There will be a dialogue box, select 'Yes'. 

####  Create password for the new app.
1. In settings for the app, select 'Keys'.
1. Under 'Passwords' section, enter a description such as 'devopskey' and select 'Save'. 
1. A value for the password will be shown, copy and paste it in a safe place. This is a sensitive piece of information.  
1. Navigate to the overview page of the app, and copy the 'Application ID'. It will be used in next steps.

### Configure yor Azure DevOps git repository
1. Sign in to your Azure DevOps organization and navigate to your project.
1. In your project, navigate to the 'files' page. 
1. Create a folder called 'B2CAssets'
1. Add your Azure AD B2C Policies here. 
1. Create another folder with name Scripts. 
1. Copy the PowerShell Script named 'DeployToB2c.ps1' from **this** sample to the newly created folder in your repo.
    1. The script gets a token from Azure AD based on the config and then calls Microsoft Graph Api to upload the policy.

### Configure your Azure DevOps release pipeline
1. Sign in to your Azure DevOps organization and navigate to your project.
1. In your project, navigate to the 'Release' page under 'Pipelines'. Then choose the action to create a new pipeline.
1. Select 'Empty Job' at the top of navigation pane to choose a template.
1. In the next screen, enter a name for the stage such as 'DeployCustomPolicies'
1. Switch to 'Variables' tab.
1. Add following variables
    1. Name: clientId, Value: 'applicationId of the app you created earlier'
    1. Name: clientSecret, Value: 'password of the app you created earlier'. 
        - Please make sure to change variable type to 'Secret' by selecting the lock icon next to Value field. 
    1. Name: tenantId, Value: 'yourtenant.onmicrosoft.com'
    
1. Switch to Tasks tab
1. Select Agent job, from right side search for 'PowerShell' and add it. 
1. Select newly added 'PowerShell Script' task.
1. Enter following values 
    1. Task Version: 1.*
    1. Type : File Path
    1. Script Path: 'Scripts/DeployToB2C.ps1'
        - this is the path to the script file you had added earlier. 
    1. Arguments: -ClientID $(clientId) -ClientSecret $(clientSecret) -TenantId $(tenantId) -PolicyId B2C_1A_TrustFrameworkBase -PathToFile B2CAssets\TrustFrameworkBase.xml



-ClientID $(clientId) -ClientSecret $(clientSecret) -TenantId $(devTenantId) -PolicyId B2C_1A_TrustFrameworkBase -PathToFile B2CAssets\BalloonsTraders.onmicrosoft.com\TrustFrameworkBase.xml
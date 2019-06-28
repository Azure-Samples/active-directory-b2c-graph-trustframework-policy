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
1. Select Azure Active Direcotry blade in the portal. Form the left menu, select App registrations (Legacy). 
1. From the command bar, select + icon which says 'New applicatin registration'.
1. Enter configuration as suggested below and select 'Create'
    1. Name :a name of your choice. Let's call it B2CDeployApp.
    1. Aplication type : Web app/API
    1. Sign-on URL : https://jwt.ms (you can choose any url of your choice).

1. After the app is created. Go to the app, select 'Settings' and then 'Required Permissions' from the menu.
1. Select 'Add'. And then 'Select an API' 
1. Select 'Microsoft Graph' from the list and then select Create.
1. Select 'Select permissions'. From **APPLICATION PERMISSIONS**, check 'Read and write your organization's trust framework policies'. 
1. Select at the bottom, and then 'Done'.
1. Select 'Grant permissions' to grant newly selected permissions consent to the app. 
1. There will be a dialogue box, select 'Yes'. 

### Create password for the new app.

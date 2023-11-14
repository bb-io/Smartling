# Blackbird.io Smartling

Blackbird is the new automation backbone for the language technology industry. Blackbird provides enterprise-scale automation and orchestration with a simple no-code/low-code platform. Blackbird enables ambitious organizations to identify, vet and automate as many processes as possible. Not just localization workflows, but any business and IT process. This repository represents an application that is deployable on Blackbird and usable inside the workflow editor.

## Introduction

<!-- begin docs -->

Smartling is a cloud-based translation and localization platform that enables businesses to efficiently manage and automate the translation of content across various languages. With advanced features and intuitive tools, Smartling ensures consistent and effective global communication strategies for enterprises of all sizes. 

## Before setting up

Before you can connect you need to make sure that:

- You have a Smartling account with [Account Owner](https://help.smartling.com/hc/en-us/articles/360056193134#h_01F5GM4FJ4Y57MYKZM0YDSM06Q) or [Project Manager](https://help.smartling.com/hc/en-us/articles/360056193134#h_01F5GM4MXQS34S0J9R10A5CNJP) permissions.
- You created a project (if you have Account Owner permissions) or the project was created for you (if you have Project Manager permissions).
- You obtained API credentials for the project. Follow [the steps to generate API v2 tokens](https://help.smartling.com/hc/en-us/articles/115004187694-API-Tokens-). After this, note **Project ID**, **User Identifier** and **User Secret** values, as they will be used to create a connection to Smartling via Blackbird.

## Connecting

1. Navigate to apps and search for Smartling. If you cannot find Smartling then click _Add App_ in the top right corner, select Smartling and add the app to your Blackbird environment.
2. Click _Add Connection_.
3. Name your connection for future reference e.g. 'My localization project'.
4. Fill in the **Project ID**, **User identifier** and **User secret** of your Smartling instance you want to connect to.
5. Click _Connect_.

![Connecting](image/README/connecting.png)

## Actions

### Jobs

- **Get job** gets the details of a job such as job name, description, due date, and reference number.
- **Create job** creates a new job.
- **Add locale to job** adds a locale to a job. Set _Sync content_ parameter to _false_ to add only a locale placeholder to the job without including all content from the job for the new locale.
- **Authorize job** authorizes all content within a job. A job can only be authorized when it has content and is in an _Awaiting Authorization (AWAITING_AUTHORIZATION)_ state. You can specify target locales and workflow for authorization. Either both the target locales and workflow should be specified, or none of them should be specified. If they are not specified, Smartling will authorize the job for the default workflows for the given project.  
- **Close job** closes a completed job. In order for a job to be closed, it must be in a completed state. All content from the job will be removed when it is closed. Closing a job guarantees that no additional work will be done against the job.
- **Cancel job** cancels a job. All content within the job will be removed from the job and the content will be unauthorized.
- **Update job** updates the attributes of the job, such as job name, description, due date, and reference number. Specify only attributes that need to be updated. Job can be edited only in _Draft (DRAFT)_, _Awaiting Authorization (AWAITING_AUTHORIZATION)_, _In Progress (IN_PROGRESS)_, _Completed (COMPLETED)_ statuses.
- **Delete job** deletes a job. Only job that is in _Cancelled (CANCELLED)_ status can be deleted.

### Job files

- **Upload source file to job** adds all non-published strings from a file to a job. The file will be added for all locales of the job if _Target locales_ parameter is not specified.
- **List files within job** lists all files within a job.
- **Download source file**.
- **Download translated file** downloads translated file for a single locale.
- **Download file translations** downloads all translations for the requested file as separate files.
- **Download file translations in ZIP archive**.

## Missing features

- Issues
- Strings
- Tags
- Reports
- Attachments
- Translation quality checks
- Glossaries
- File Machine Translations
- Context

Let us know if you're interested!

## Feedback

Feedback to our implementation of Smartling is always very welcome. Reach out to us using the [established channels](https://www.blackbird.io/) or create an issue.

<!-- end docs -->

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
- **Search jobs** lists jobs that match the specified filter options. If no parameters are specified, all jobs will be returned.
- **Create job** creates a new job. Leave the _Callback URL_ and _Callback method_ parameters unspecified if you intend to use **On job completed** and/or **On job cancelled** events.
- **Add locale to job** adds a locale to a job. Set _Sync content_ parameter to _false_ to add only a locale placeholder to the job without including all content from the job for the new locale.
- **Authorize job** authorizes all content within a job. A job can only be authorized when it has content and is in an _Awaiting Authorization (AWAITING_AUTHORIZATION)_ state. You can specify target locales and workflow for authorization. Either both the target locales and workflow should be specified, or none of them should be specified. If they are not specified, Smartling will authorize the job for the default workflows for the given project.  
- **Close job** closes a completed job. In order for a job to be closed, it must be in a completed state. All content from the job will be removed when it is closed. Closing a job guarantees that no additional work will be done against the job.
- **Cancel job** cancels a job. All content within the job will be removed from the job and the content will be unauthorized.
- **Update job** updates the attributes of the job, such as job name, description, due date, and reference number. Specify only attributes that need to be updated. Job can be edited only in _Draft (DRAFT)_, _Awaiting Authorization (AWAITING_AUTHORIZATION)_, _In Progress (IN_PROGRESS)_, _Completed (COMPLETED)_ statuses. Leave the _Callback URL_ and _Callback method_ parameters unspecified if you intend to use **On job completed** and/or **On job cancelled** events.
- **Delete job** deletes a job. Only job that is in _Cancelled (CANCELLED)_ status can be deleted.

### Job files

- **Upload source file to job** adds all non-published strings from a file to a job. The file will be added for all locales of the job if _Target locales_ parameter is not specified.
- **List source files within job** lists all source files within a job.
- **Download source file**.
- **Download translated file** downloads translated file for a single locale.
- **Download file translations** downloads all translations for the requested file as separate files.
- **Download file translations in ZIP archive**.
- **Import translation** imports a translated file. This action supports a limited range of file types. You can find information about supported file types and file preparation [here](https://help.smartling.com/hc/en-us/articles/360008031794-Importing-Translated-Files).

### Job attachments

- **List files attached to job**.
- **Upload attachment to job**.
- **Download file attached to job**.

### Strings

- **Add string to project** uploads a string to a project. There are optional _Placeholder format_ and _Custom placeholder format (Java Regular Expression)_ parameters which are used to specify a standard or custom placeholder format. Placeholders are words in a string that should not be translated. They represent dynamic variable content. You can read more about placeholders [here](https://help.smartling.com/hc/en-us/articles/360008143433). Optional _Namespace_ parameter is used to generate the unique hashcode for a given string. If you want to have two versions of a string with the same text and variant metadata, you can keep them unique by assigning a different namespace to each string. Optional _Maximum character length_ parameter can be used to set the maximum character length recommended for this string and its translations. Leave the _Callback URL_ and _Callback method_ parameters unspecified if you intend to use **On string translation published** event.
- **Add string to job** adds a string to a job. In order to be able to add a string to a job, it first needs to be added to project via **Add string to project** action. A string for a locale can only be in one job. If the string identified in the request is already in a job for a locale then it won't be added. However, if you set _Move enabled_ to _True_, the string will be moved into the specified job.
- **Remove string from job**.
- **List all source strings for file**.
- **Get source string by hashcode** retrieves a single source string with a specified hashcode, which is the unique identifier for the string.
- **List strings in file**.
- **List translations for strings in file**.
- **List translations for string by hashcode**.

**Note**: A string added with **Add string to project** action can be authorized by a content owner in the Smartling Dashboard. Another way to authorize the string is to add it to a job with **Add string to job** action and then authorize the job with **Authorize job** action.

### Issues

- **Get issue** retrieves detailed information about a single issue.
- **Search issues** lists issues that match the specified filter options. If no parameters are specified, all issues will be returned.
- **Create issue** creates a new issue for a string. Maximum length of _Issue text_ is 4000 characters. _Target language_ parameter is required for issues of the _Translation_ type. 
- **Edit issue**. Specify only fields that need to be updated. Maximum length of _Issue text_ is 4000 characters. _Target language_ parameter is required when updating an issue to have the _Translation_ type. _Issue subtype_ parameter is required when updating _Issue type_.
- **Open issue** sets the state of an issue to _Opened_.
- **Close issue** sets the state of an issue to _Resolved_.

### Glossaries

- **Get glossary** retrieves detailed information about a single glossary.
- **Create glossary**.
- **Update glossary** updates an existing glossary. Specify only fields that need to be updated. Specifying the _Locales_ parameter overrides the existing locales. To simply add more locales to the existing list, use the **Add locales to glossary** action.
- **Add locales to glossary**.
- **Add fallback locale to glossary**. You can read about fallback locales [here](https://help.smartling.com/hc/en-us/articles/4408673919643).
- **Import glossary** allows you to import a glossary, creating a new one, or importing data into an existing glossary. If you wish to import data into an existing glossary, you should set an optional _Glossary ID_ input parameter. This action does not override existing data; instead, it adds new entries to the existing glossary. You can optionally set the _Archive existing entries_ input parameter to _True_ to archive entries that are not in the import file.
- **Export glossary** allows you to export an existing glossary. You can apply optional input parameters to filter the entries included in the export.
- **Get glossary entry**.
- **Search glossary entries** lists glossary entries that match the specified filter options. If no parameters are specified, all glossary entries will be returned.
- **Create glossary entry** creates a glossary entry along with its definition and original term.
- **Update glossary entry**. Specify only fields that need to be updated.
- **Add glossary entry translation** adds or updates glossary entry translation. If there is no translation for the specified _Locale_, a new translation is added. Otherwise, the existing translation is updated, but only with the specified parameters; all other parameters remain unchanged.
- **Remove glossary entry**.

### Reports

- **Get word count report**.
- **Get word count report in CSV format**.

## Events

- **On job completed** and **On job completed (manual)** are triggered when a job is completed, signifying that all authorized content in a job, for all locales, has reached the _Published_ step of the workflow.
- **On job cancelled** and **On job cancelled (manual)** are triggered when a job is cancelled.
- **On string translation published** and **On string translation published (manual)** are triggered when a string translation is published for a locale.

## Missing features

- Issue comments
- Issue webhooks
- Tags
- Translation quality checks
- File Machine Translations
- Context
- Custom fields

Let us know if you're interested!

## Feedback

Feedback to our implementation of Smartling is always very welcome. Reach out to us using the [established channels](https://www.blackbird.io/) or create an issue.

<!-- end docs -->

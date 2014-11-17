# Buddy .NET SDK
These release notes are for the Buddy Platform .NET SDK.

Please refer to [buddyplatform.com/docs](https://buddyplatform.com/docs#NETSDK) for more details on the .NET SDK.

## Introduction

We realized most developers end up writing the same code over and over again: user management, photo management, geolocation, checkins, metadata, and other common features. Buddy enables developers to build cloud-connected apps without having to write, test, manage or scale server-side code and infrastructure.

Buddy's scenario-focused APIs let you spend more time building your application and less time worrying about backend infrastructure.

The .NET SDK handles the following functionality:

* Building and formatting requests
* Managing authentication
* Parsing responses
* Loading and saving credentials

## Getting Started

To get started with the Buddy Platform SDK, please reference the _Getting Started_ series of documents at [buddyplatform.com/docs](https://buddyplatform.com/docs). You will need an application ID and key before you can use the SDK. The _Getting Started_ documents will walk you through obtaining everything you need and show you where to find the SDK for your platform.

Application IDs and keys are obtained at the Buddy Developer Dashboard at [buddyplatform.com](https://buddyplatform.com/login).

Full documentation for Buddy's services are available at [buddyplatform.com/docs](https://buddyplatform.com/docs).

#### Visual Studio - NuGet

We recommend using NuGet to include the SDK in your project. To include the Buddy .NET SDK in your project:
* Right-click on your project in Visual Studio and select the "Manage NuGet Packages..." menu item
* Type "BuddyPlatformSdk" into the edit box in the upper-right-hand corner of the "Manage NuGet Packages" window
* Click "Install"

##### Add A Reference

You must add a reference to the Buddy SDK in order to use it in your application.
* Open your project in Visual Studio
* In the Solution Explorer, right-click on your project and select "Add Reference..." (about halfway down the menu)
* In the Reference Manager, click the "Browse..." button in the lower-right-hand corner of the window
* In the "Select the files to reference..." dialog, find the assembly file for your project type and click "Add"
* Click "OK" in the "Reference Manager" window

## Using the .NET SDK

Visit the [Buddy Dashboard](https://buddyplatform.com) to obtain your application ID and key.

### Initialize the SDK

To reference the Buddy SDK in your source file, you need to put a 'using' keyword at the top of the file that contains the constructor:

    using BuddySDK;

The `Init` method should be called once at the start of your app; we recommend placing it in your project's constructor. You should replace "your app ID" and "your app key" with the app ID and key you created at the [Buddy Dev Dashboard](http://buddyplatform.com):

    public MyCoolApp()
    {
        // Don't forget to get your app ID and key from http://buddyplatform.com!
        Buddy.Init("your app ID", "your app key");
     }

### User Flow

The Buddy .NET SDK handles user creation and login.

#### Create A User

    // We recommend awaiting user creation, login, and logout
    await Buddy.CreateUserAsync(username, password);

#### User Login

    var result = await Buddy.LoginUserAsync(username, password);
    // Obtain the user's userName with result.Value.userName;

#### User Logout

    await Buddy.LogoutUserAsync();

### REST Interface

Each SDK provides general wrappers that make REST calls to Buddy.

#### POST

In this example we'll create a checkin. Take a look at the [create checkin REST documentation](https://buddyplatform.com/docs/Checkins#CreateCheckin) then:

    var result = await Buddy.PostAsync<Checkin>("/checkins", new {
                            location = new BuddyGeoLocation(47.1, -122.3),
                            comment =  "This place was awesome!"
                        });
    // POST results return similar responses to GET, use the result to check for success

#### GET

We now can call GET to [search for the checkin](https://buddyplatform.com/docs/Checkins#SearchCheckins) we just created!

    // GET all checkins within a 5000 meter radius around the point 47.1, -122.3
    var result = await Buddy.GetAsync<PagedResult<Checkin>>("/checkins", new { locationRange = "47.1,-122.3,5000" } );

#### PUT/PATCH/DELETE

Each remaining REST verb is available through the Buddy SDK using the same pattern as the POST and GET examples.

### Working With Files

Buddy offers support for binary files. The .NET SDK works with files through our REST interface similarly to other API calls.

**Note:** Responses for files deviate from the standard Buddy response templates. See the [Buddy Platform documentation](https://buddyplatform.com/docs) for more information.

#### Upload A File

Here we demonstrate uploading a picture. For all binary files (e.g. blobs and videos), the pattern is the same, but with a different path and different parameters. For full documentation see our [Media and Files](https://buddyplatform.com/docs/Media%20and%20Files) documentation page.

    // Create a new BuddyFile with the picture we want to upload
    var result = await Buddy.PostAsync<Picture> ("/pictures", new {
                            data = new BuddyFile (chosenImage.AsPNG().AsStream(), "data", "image/png"),
                        });

#### Download A File

Our download example uses pictures.

    // Gets the photo bits, resized to 200x200
    var result = await Buddy.GetAsync<BuddyFile>("/pictures/" + id + "/file", new {size=200});

    if (result.IsSuccess && result.Value != null) {
        
        // Load the image data from result.Value.Data
    }

## Contributing Back: Pull Requests

We'd love to have your help making the Buddy SDK as good as it can be!

To submit a change to the Buddy SDK please do the following:

1. Create your own fork of the Buddy SDK
2. Make the change to your fork
3. Before creating your pull request, please sync your repository to the current state of the parent repository: `git pull origin master`
4. Commit your changes, then [submit a pull request](https://help.github.com/articles/using-pull-requests) for just that commit

## License

#### Copyright (C) 2014 Buddy Platform, Inc.

Licensed under the Apache License, Version 2.0 (the "License"); you may not
use this file except in compliance with the License. You may obtain a copy of
the License at

  [http://www.apache.org/licenses/LICENSE-2.0](http://www.apache.org/licenses/LICENSE-2.0)

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
License for the specific language governing permissions and limitations under
the License.

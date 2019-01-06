# WeTransfer.NET [![Build Status](https://travis-ci.com/CodeMyst/WeTransfer.NET.svg?branch=master)](https://travis-ci.com/CodeMyst/WeTransfer.NET) [![Nuget Package](https://img.shields.io/nuget/v/WeTransfer.NET.svg)](https://www.nuget.org/packages/WeTransfer.NET/)

A .NET Standard library that is a very basic wrapper over the [WeTransfer](https://developers.wetransfer.com/) API. This library only implements the Transfer API, there are no plans yet for implementing the Boards API.

## Usage

The easiest way to get started is to download the nuget package from here: [https://www.nuget.org/packages/WeTransfer.NET/](https://www.nuget.org/packages/WeTransfer.NET/)

It's very simple to start uploading files. First you need to get your WeTransfer API from [here](https://developers.wetransfer.com/). Here is a small sample program that will upload an image:

```cs
WeTransferClient wt = new WeTransferClient ("your api key here");

// Authorize your WeTransfer app, this call the /authorize endpoint
await wt.Authorize ();

// Create Partial File Information so WeTransfer knows how many files
// you're going to upload, the names of those files and their sizes
PartialFileInfo [] partialFileInfos = new PartialFileInfo [1];
partialFileInfos [0] = new PartialFileInfo
{
    Name = "TestFile.png",
    Path = "/path/to/the/file/TestFile.png",
    // Size of the file in bytes
    Size = 48570239
};

// Create a File Transfer which informs WeTransfer that you're about to upload files
// The second parameter is the transfer message which will show on the download page
FileTransferResponse response = await wt.CreateTransfer (partialFileInfos,
                                                         "Test Transfer");

// Now you can upload the files!
// The first parameter is the transfer's ID
await wt.Upload (response.ID, response.Files);

// Now you need to tell WeTransfer that your files have been uploaded
FileUploadResult result = await wt.FinalizeUpload (response.ID, response.Files);

// FileUploadResult contains the url to the download page and the date of the expiry
```

## Building

If you want to build this from source all you need is .Net SDK and run the `dotnet build` command.

## Unit Tests

This project also includes unit tests (just one for now).

First you **have** to have the `WE_TRANSFER_KEY` environment variable set with your API key.

All you need to run the tests is run `dotnet test` which will upload the [TestFile.png](test/TestFile.png) image (around 16MB, it's that big to make sure that multipart uploading works as it should).

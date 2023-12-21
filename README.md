# Chirp for BDSA 2023

## About
This is the repository for our web application, <i>Chirp!</i> , developed as part of our course, <b>Analysis, Design and Software Architecture.</b>

## Developers
The developers, known as <b>Group 17</b>, is from the <i>IT-University of Copenhagen</i>, and consists of following members: 

- Burak Özdemir (buoe)
- Hanan Choudhary Hadayat (abha)
- Joshua James Medilo Calba (jcal)
- Julius Dalsgaard Bertelsen (jube)
- Tan Dang (tanda)

### Further details</h3>
A full description of the application and the development process is available at the following PDF: 

[docs/2023_itubdsa_group_17_report.pdf](https://github.com/ITU-BDSA23-GROUP17/Chirp/blob/main/docs/2023_itubdsa_group_17_report.pdf)

The PDF is available in the docs folder of this repository.

## How to run

### Access the official web-app
The official web-app will be available on the following link: [chirp17](https://bdsagroup17chirprazor.azurewebsites.net/)

### Run locally

In order to run the application locally, you can either:

1. clone this repository
2. run the release version.

#### Pre requirements  

You need to have dotnet 7.0 installed see [download](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)

You need to have setup AzureB2C Tenant see the [guide](https://learn.microsoft.com/en-us/azure/active-directory-b2c/tutorial-create-tenant)

You have docker installed see [docker](https://www.docker.com/products/docker-desktop/)

#### Cloned repository</h4>
In order to run the application locally by cloning the repository, please do as follows:

 Clone the repository using this git command:
 ```
 git clone https://github.com/ITU-BDSA23-GROUP17/Chirp.git
 ```
 Change directory into 
  ```
  cd "src/Chirp.Web"
  ```
 Inside the directory, run <b>one</b> of the following commands: </li>
  ```
  dotnet watch --clientsecret [your-secret]
  ```
  ```
  dotnet run --clientsecret [your-secret]
  ``` 
  
  You should now have access to a localhost with a specific port, in which this web-app can be accessed


#### Releases
In order to run the release versions, please do as follows:

- On the main page of this repository, click on the <b>Releases</b>-section</li>
There will be a few assets available (including source code), but only one of the following three will be relevant for us:</li>
 
  - Chirp-win-x64.zip</i>, for Windows users</li>  
  - Chirp-osx-x64.zip</i>, for Mac users</li>
  - Chirp-linux-x64.zip</i>, for Linux users</li>  <br>
    
  Please install and unzip one of the three folders, depending on your operating system</li>
  Now, there should be the following application available in the extracted folder:</li><br>

    - Chirp.Web.exe</i>, for Windows users</li>  
    - Chirp.Web</i>, for Mac and Linux users</li> <br>
  

  Now, you have an runnable (as described in step 4). Depending on your operating system, you can run the web-app as follows: </li>

  Run the following commands:
     ```
     dotnet dev-certs https -t
     ```

     ```
     ./Chirp.Web --urls="https://localhost:7102;http://localhost:5273" --clientsecret [your-secret]
     ```
       
  Upon running the application, a terminal will pop up, indicating in which port (in the localhost) the web-app is up and running

## Other

### For developers:

-If you are adding a feature, create a branch feat/<feat-name>

-If you are fixing an issue, create a branch fix/<fix-name>

-If you are testing a feature, create a branch testing/<feat-name>

-If a version is old, create a branch deprecated/<old-deprecated-version-name>



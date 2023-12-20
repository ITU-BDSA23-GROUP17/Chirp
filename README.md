<h1>Chirp for BDSA 2023</h1>

<h2>About</h2>
This is the repository for our web application, <i>Chirp!</i> , developed as part of our course, <b>Analysis, Design and Software Architecture.</b>

<h3>Developers</h3>
The developers, known as <b>Group 17</b>, is from the <i>IT-University of Copenhagen</i>, and consists of following members: 

<ul>
  <li>Burak Özdemir (buoe)</li>
  <li>Hanan Choudhary Hadayat (abha)</li>
  <li>Joshua James Medilo Calba (jcal)</li>
  <li>Julius Dalsgaard Bertelsen (jube)</li>
  <li>Tan Dang (tanda)</li>
</ul>

<h3>Further details</h3>
A full description of the application and the development process is available at the following PDF: <b>2023_itubdsa_group_17_report.pdf</b><br>
The PDF is available at the root of this repository.

<h2>How to run</h2>
<h3>Access the official web-app</h3>
The official web-app will be available on the following link: <a href="bdsagroup17chirprazor.azurewebsites.net/">bdsagroup17chirprazor.azurewebsites.net</a>

<h3>Run locally</h3>
In order to run the application locally, you can either <b>1. clone this repository</b>, or <b>2. run the release version</b>.

<h4>Cloned repository</h4>
In order to run the application locally by cloning the repository, please do as follows:
<ol>
  <li>Clone the repository using this git command: <i>git clone https://github.com/ITU-BDSA23-GROUP17/Chirp/</i> </li>
  <li>Change directory into <i>"src/Chirp.Web"</i></li>
  <li>Inside the directory, run <b>one</b> of the following commands: </li>
  <ul>
    <li><i>dotnet watch --launch-profile https</i></li>  
    <li><i>dotnet run</i></li>    
  </ul>
  <li>You should now have access to a localhost with a specific port, in which this web-app can be accessed</li>
</ol> 

<h4>Releases</h4>
In order to run the release versions, please do as follows:
<ol>
  <li>On the main page of this repository, click on the <b>Releases</b>-section</li>
  <li>There will be a few assets available (including source code), but only one of the following three will be relevant for us:</li>
  <ul>
    <li><i>Chirp-win-x64.zip</i>, for Windows users</li>  
    <li><i>Chirp-osx-x64.zip</i>, for Mac users</li>
    <li><i>Chirp-linux-x64.zip</i>, for Linux users</li>  
  </ul>
  <li>Please install and unzip one of the three folders, depending on your operating system</li>
  <li>Now, there should be the following application available in the extracted folder:</li>
  <ul>
    <li><i>Chirp.Web.exe</i>, for Windows users</li>  
    <li><i>Chirp.Web</i>, for Mac and Linux users</li>
  </ul>
  <li>Now, you have an runnable (as described in step 4). Depending on your operating system, you can run the web-app as follows: </li>
  <ul>
    <li><b>(Mac and Linux users):</b> Run the following command: <i>./Chirp.Web</i></li>
    <li><b>(Windows users):</b> Just double-click on the following executable: <i>Chirp.Web.exe</i></li>
  </ul>
  <li>Upon running the application, a terminal will pop up, indicating in which port (in the localhost) the web-app is up and running</li>
</ol> 

<h2>Other</h2>
<h3>For developers:</h3>

-If you are adding a feature, create a branch feat/<feat-name>

-If you are fixing an issue, create a branch fix/<fix-name>

-If you are testing a feature, create a branch testing/<feat-name>

-If a version is old, create a branch deprecated/<old-deprecated-version-name>



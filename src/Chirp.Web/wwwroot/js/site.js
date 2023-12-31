const button = document.querySelector("#menu-button");
const menu = document.querySelector("#menu");

button.addEventListener("click", () => {
  menu.classList.toggle("hidden");
});

// ask user if they arer sure they want to delete their profile
function confirmForget() {
  var confirmResult = window.confirm("Are you sure? This action cannot be undone!");

  return confirmResult; 
}

// parts of function shortened thanks to ChatGPT
function showWordsLeft() {
  var cheepTextAreaElement = document.getElementById("cheepTextArea");
  var charactersLeftElement = document.getElementById("charactersLeft");
  var cheepButtonElement = document.getElementById("cheepButton");
  var textFromTextArea = cheepTextAreaElement.value;

  cheepTextAreaElement.addEventListener("input", () => {
    if (textFromTextArea.length > 0) {
      cheepButtonElement.classList.remove("text-red", "border-red", "hover:bg-red-200");
      cheepButtonElement.classList.add("text-black", "border-black", "hover:bg-black");
      cheepButtonElement.disabled = false;
    } else {
      cheepButtonElement.classList.remove("text-black", "border-black", "hover:bg-black");
      cheepButtonElement.classList.add("text-red", "border-red", "hover:bg-red-200");
      cheepButtonElement.disabled = true;
    }
  });

  cheepTextAreaElement.addEventListener("change", () => {
    if (textFromTextArea.length > 0) {
      cheepButtonElement.classList.remove("text-red", "border-red", "hover:bg-red-200");
      cheepButtonElement.classList.add("text-black", "border-black", "hover:bg-black");
      cheepButtonElement.disabled = false;
    } else {
      cheepButtonElement.classList.remove("text-black", "border-black", "hover:bg-black");
      cheepButtonElement.classList.add("text-red", "border-red", "hover:bg-red-200");
      cheepButtonElement.disabled = true;
    }
  });

  charactersLeftElement.textContent =
    "Character left: " + (160 - textFromTextArea.length);
}

// Makes sure 'Cheep!' button can't be tapped again while submitting cheep
function cheepDisableMultiClick(){
  var cheepTextAreaElement = document.getElementById("cheepTextArea");
  var cheepButtonElement = document.getElementById("cheepButton");
  var textFromTextArea = cheepTextAreaElement.value;

  if (textFromTextArea.length > 0) {
    cheepButtonElement.disabled = true;
    return true;
  } else {
    return false;
  }
}

// TO UPLOAD profile pics
const inputDiv = document.getElementById("input-div");
const uploadButton = document.getElementById("upload-button");

inputDiv.addEventListener("dragover", (e) => {
  e.preventDefault();
  inputDiv.classList.add("border-blue-500");
});

inputDiv.addEventListener("dragleave", () => {
  inputDiv.classList.remove("border-blue-500");
});

inputDiv.addEventListener("drop", (e) => {
  e.preventDefault();
  inputDiv.classList.remove("border-blue-500");

  const files = e.dataTransfer.files;

  console.log("Dropped files:", files);
});

inputDiv.addEventListener("dragenter", (e) => {
  e.preventDefault();
});

const fileSelect = document.getElementById("fileSelect");
const fileElem = document.getElementById("fileElem");

fileSelect.addEventListener(
  "click",
  (e) => {
    if (fileElem) {
      fileElem.click();
    }
    const files = fileElem.files;
    uploadButton.classlist.remove("hidden");
    console.log("Selected files:", files);
  },
  false
);

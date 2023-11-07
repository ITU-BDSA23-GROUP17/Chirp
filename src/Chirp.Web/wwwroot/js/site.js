const button = document.querySelector("#menu-button");
const menu = document.querySelector("#menu");

button.addEventListener("click", () => {
  menu.classList.toggle("hidden");
});

// function shortened thanks to ChatGPT
function showWordsLeft() {
  var cheepTextAreaElement = document.getElementById("cheepTextArea");
  var charactersLeftElement = document.getElementById("charactersLeft");
  var cheepButtonElement = document.getElementById("cheepButton");
  var textFromTextArea = cheepTextAreaElement.value;

  cheepTextAreaElement.addEventListener("input", () => {
    if (textFromTextArea.length > 0) {
      cheepButtonElement.disabled = false;
    } else {
      cheepButtonElement.disabled = true;
    }
  });

  charactersLeftElement.textContent =
    "Character left: " + (160 - textFromTextArea.length);
}

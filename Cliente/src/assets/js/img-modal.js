var modal = document.getElementById("modal-custom");
var image = document.getElementsByClassName("img-custom")[0];
var imgModal = document.getElementById("img-inside-modal");
var spanClose = document.getElementsByClassName("img-close")[0];
var btnFile = document.getElementById("btnFile");

image.onclick = function () {
  modal.style.display = "block";
  btnFile.style.display = "none";
  imgModal.src = this.src;
}
spanClose.onclick = function () {
  modal.style.display = "none";
  btnFile.style.display = "block";
}
document.addEventListener('DOMContentLoaded', function () {
    const colorInput = document.getElementById('color');
    const colorPreview = document.querySelector('.color-preview');
    const colorDisplay = document.getElementById('colorDisplay');
    const colorPicker = document.getElementById('colorPicker');

    // Open color picker when clicking on the preview or text input
    colorPreview.addEventListener('click', function () {
        colorPicker.click();
    });

    colorDisplay.addEventListener('click', function () {
        colorPicker.click();
    });

    // Update all values when color is selected
    colorPicker.addEventListener('input', function (e) {
        const selectedColor = e.target.value;
        colorInput.value = selectedColor;
        colorPreview.style.backgroundColor = selectedColor;
        colorDisplay.value = selectedColor;
    });
});
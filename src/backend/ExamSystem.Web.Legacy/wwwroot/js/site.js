function addTogglePasswordVisibilityHandler(buttonId, inputId) {
    document.getElementById(buttonId).addEventListener('click', function (event) {
        event.preventDefault();
        const passwordInput = document.getElementById(inputId);
        if (passwordInput.type === "password") {
            passwordInput.type = "text";
        } else {
            passwordInput.type = "password";
        }
    });
}

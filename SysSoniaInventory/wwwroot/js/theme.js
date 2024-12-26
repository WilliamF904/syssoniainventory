

    document.addEventListener("DOMContentLoaded", function () {
            const toggleThemeButton = document.getElementById("toggle-theme");
    const body = document.body;

    // Determinar el estado inicial (oscuro por defecto)
    let isDarkMode = true;

            toggleThemeButton.addEventListener("click", () => {
                if (isDarkMode) {
        body.classList.remove("dark-theme");
    body.classList.add("light-theme");
    toggleThemeButton.textContent = "Cambiar a Tema Oscuro";
    toggleThemeButton.classList.remove("btn-dark");
    toggleThemeButton.classList.add("btn-light");
                } else {
        body.classList.remove("light-theme");
    body.classList.add("dark-theme");
    toggleThemeButton.textContent = "Cambiar a Tema Claro";
    toggleThemeButton.classList.remove("btn-light");
    toggleThemeButton.classList.add("btn-dark");
                }
    isDarkMode = !isDarkMode;
            });
        });


    document.addEventListener("DOMContentLoaded", function () {
            const darkModeButton = document.getElementById("sidebar-dark-theme");
    const lightModeButton = document.getElementById("sidebar-default-theme");
    const body = document.body;

            darkModeButton.addEventListener("click", () => {
        body.classList.remove("light-theme");
    body.classList.add("dark-theme");
            });

            lightModeButton.addEventListener("click", () => {
        body.classList.remove("dark-theme");
    body.classList.add("light-theme");
            });
        });


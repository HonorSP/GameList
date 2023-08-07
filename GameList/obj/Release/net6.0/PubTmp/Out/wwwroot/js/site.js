// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//function toggleTheme() {
//    // Get current theme from cookie
//    var theme = getCookie("theme");
//    if (theme == "light") {
//        theme = "dark";
//    } else {
//        theme = "light";
//    }
//    // Set new theme in cookie
//    setCookie("theme", theme, 365);
//    // Update styles on the page to reflect new theme
//    if (theme == "light") {
//        document.body.style.backgroundColor = "#fff";
//        document.body.style.color = "#000";
//    } else {
//        document.body.style.backgroundColor = "#000";
//        document.body.style.color = "#fff";
//    }
//}

//function getCookie(name) {
//    var value = "; " + document.cookie;
//    var parts = value.split("; " + name + "=");
//    if (parts.length == 2) return parts.pop().split(";").shift();
//}


//function setCookie(name, value, days) {
//    var expires = "";
//    if (days) {
//        var date = new Date();
//        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
//        expires = "; expires=" + date.toUTCString();
//    }
//    document.cookie = name + "=" + (value || "") + expires + "; path=/";
//}



//document.addEventListener("DOMContentLoaded", function () {
//    var themeSwitch = document.getElementById("theme-switch");
//    var theme = localStorage.getItem("theme");
//    if (theme === "dark") {
//        themeSwitch.checked = true;
//        document.getElementById("stylesheet").setAttribute("href", "/css/dark-theme.css");
//    } else {
//        themeSwitch.checked = false;
//        document.getElementById("stylesheet").setAttribute("href", "/css/light-theme.css");
//    }
//    themeSwitch.addEventListener("change", function () {
//        if (this.checked) {
//            document.getElementById("stylesheet").setAttribute("href", "/css/dark-theme.css");
//            localStorage.setItem("theme", "dark");
//        } else {
//            document.getElementById("stylesheet").setAttribute("href", "/css/light-theme.css");
//            localStorage.setItem("theme", "light");
//        }
//    });
//});


//document.addEventListener("DOMContentLoaded", function () {
//    var themeSwitch = document.getElementById("theme-switch");
//    var stylesheet = document.getElementById("stylesheet");
//    var stylesheetDark = document.getElementById("stylesheet-dark");
//    var theme = localStorage.getItem("theme");
//    if (theme === "dark") {
//        themeSwitch.checked = true;
//        stylesheet.disabled = true;
//        stylesheetDark.disabled = false;
//    } else {
//        themeSwitch.checked = false;
//        stylesheet.disabled = false;
//        stylesheetDark.disabled = true;
//    }
//    themeSwitch.addEventListener("change", function () {
//        if (this.checked) {
//            stylesheet.disabled = true;
//            stylesheetDark.disabled = false;
//            localStorage.setItem("theme", "dark");
//            document.cookie = "theme=dark";
//        } else {
//            stylesheet.disabled = false;
//            stylesheetDark.disabled = true;
//            localStorage.setItem("theme", "light");
//            document.cookie = "theme=light";
//        }
//    });
//});





document.addEventListener("DOMContentLoaded", function () {
    console.log("DOM fully loaded and parsed");
    if (!document.cookie.includes("CookieConsent=true")) {
        console.log("CookieConsent not found, showing banner");
        document.getElementById("cookieConsent").style.display = "block";
    }

    document.getElementById("acceptCookies").addEventListener("click", function () {
        console.log("Accept button clicked");
        document.cookie = "CookieConsent=true; path=/; max-age=" + (365 * 24 * 60 * 60) + "; samesite=lax; secure"; // 1 year
        document.getElementById("cookieConsent").style.display = "none";
    });
});

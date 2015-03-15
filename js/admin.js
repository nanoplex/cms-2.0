(function () {
    "use strict";

    var page = document.getElementById("page");

    page.Site = undefined;
    page.Authed = false;

    page.ready = function () {

    };

    page.HandleAuth = function (res) {

        var auth = JSON.parse(res.detail.response.toLowerCase());

        if (auth) {
            this.Authed = true;
            page.$.ajaxSite.go();
        }
        else page.$.login.removeAttribute("hidden");

    };

    page.Login = function () {

        var email = document.querySelector("input[name=email]"),
            pass = document.querySelector("input[name=password]");

        if (email.validity.valid && pass.validity.valid) {
            page.$.ajaxLogin.go();
            page.$.status.innerHTML = "<paper-spinner active></paper-spinner>";
        } else page.$.status.innerHTML = "you must enter valid email and a password";

    };

    page.HandleLogin = function (res) {

        if (res.detail.response.match(/true/) != null) page.$.ajaxAuth.go();
        else {
            page.$.status.innerHTML = res.detail.response;
            page.$.status.removeAttribute("hidden");
        }

    };

    page.HandleSite = function (res) {

        var site = res.detail.response;

        if (site.Name == "Site does not exist") window.location.href = "/install.html";
        else if (!this.Authed) page.$.ajaxAuth.go();
        else {
            page.$.login.setAttribute("hidden", "");
            page.Site = site;
        }

    };

    page.SelectPage = function (event, a, e) {
        page.SelectedPage = e.innerHTML;
    };

})();
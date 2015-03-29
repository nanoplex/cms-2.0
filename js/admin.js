(function () {

    "use strict";

    var page = document.getElementById("page"),
        loginStatus;

    page.SelectedPage = "view-unloaded";

    function setTitle() {
        var title = document.querySelector("title");
        page.injectBoundHTML(page.SelectedPage.replace(/^view-/, '') + " - Admin", title);
    };

    page.ready = function () {
    };

    page.handleAuth = function (res) {
        res = res.detail.response;

        if (res === "True") {
            page.$.ajaxSite.go();
        }
        else if (res === "False") {
            page.SelectedPage = "view-login";
            setTitle();
        }
    };

    page.handleLogin = function (res) {
        res = res.detail.response;

        if (res === "true") {
            page.email = null;
            page.password = null;

            page.$.ajaxAuth.go();
        }
        else {
            loginStatus.innerHTML = res;
        }
    };
    page.login = function () {
        loginStatus = document.querySelector("section[data-pagename=view-login] .status");
        page.injectBoundHTML("<paper-spinner active><paper-spinner/>", loginStatus);

        page.$.ajaxLogin.go();
    };

    page.handleSite = function (res) {
        res = res.detail.response;

        document.querySelector("header").removeAttribute("hidden");
        document.getElementById("add").removeAttribute("hidden");

        if (res !== null) {
            page.Site = res;
            page.SelectedPage = page.Site.Pages[0].Name;
            setTitle();

            console.log(page.Site);
        }
    };

    var click = false,
        end = false,
        Delete = false,
        startPos,
        pos;

    page.startPageSelect = function (id) {
        click = false;
        end = false;

        document.querySelector("core-drawer-panel").setAttribute("disableSwipe", "true");

        setTimeout(function () {
            if (!click) {
                var self = document.querySelector("nav section[data-id='" + id + "']");

                startPos = undefined;
                end = true;

                self.setAttribute("class", "drag");

                window.onmousemove = function (event) {
                    if (startPos == undefined)
                        startPos = event.clientY;

                    pos = event.clientY;

                    self.setAttribute("style", "top:" + ((pos - startPos)) + "px");
                };
            }
        }, 500);
    };
    page.endPageSelect = function (id, name, order) {

        if (!end) {


            page.SelectedPage = name;
            setTitle();

            click = true;
        }
        else {
            window.onmousemove = undefined;

            var move = pos - startPos,
                height = document.querySelectorAll("nav section[data-id]")[0].offsetHeight,
                newOrder = Math.round(order + (move / -height));

            (newOrder > order)
                ? newOrder = (newOrder + 1)
                : newOrder = (newOrder - 1);

            page.$.ajaxEditPage.params = '{"id":"' + id + '","name":"' + name + '","order":' + newOrder + '}';
            page.$.ajaxEditPage.go();
        }

        document.querySelector("core-drawer-panel").setAttribute("disableSwipe", "false");
    };

    page.deletePage = function (id) {
        page.$.ajaxDeletePage.params = '{"id":"' + id + '"}';
        page.$.ajaxDeletePage.go();
        Delete = false;
    };
    page.addPage = function () {
        var firstNavEl = document.querySelector('nav section:nth-child(4)');

        console.log(firstNavEl.getAttribute("data-order"));

        page.$.ajaxAddPage.params = '{"name":"' + page.newPageName + '","order":' + (parseInt(firstNavEl.getAttribute("data-order"), 10) + 1) + '}';
        page.$.ajaxAddPage.go();

        page.newPageName = null;
    };
    page.showAddPage = function () {
        page.SelectedPage = "view-add-page";
        setTitle();
    };

    page.toggleNav = function () {
        document.querySelector("core-drawer-panel").togglePanel();
    };

    page.toggleAddOptions = function () {
        if (page.$.addOptions.getAttribute("hidden") == null)
            page.$.addOptions.setAttribute("hidden", "");
        else page.$.addOptions.removeAttribute("hidden");
    };

    page.showAddComponent = function () {
        page.$.addOptions.setAttribute("hidden", "");

        page.SelectedComponent = "view-add-component";
        setTitle();
    };
})();
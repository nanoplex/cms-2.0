﻿(function () {

    "use strict";

    var page = document.getElementById("page"),
        loginStatus;

    page.ready = function () {
        page.changeView("view-loading");
    };

    page.handleAuth = function (res) {
        res = res.detail.response;
        page.changeView("view-loading");

        if (res === "True")
            page.$.ajaxSite.go();
        else if (res === "False") {
            page.changeView("view-login");
        }
    };

    page.handleLogin = function (res) {
        res = res.detail.response;

        if (res === "true") {
            page.email = null;
            page.password = null;

            page.$.ajaxAuth.go();
        }
        else loginStatus.innerHTML = res;
    };


    page.handleSite = function (res) {
        res = res.detail.response;

        document.querySelector("header").removeAttribute("hidden");
        document.getElementById("add").removeAttribute("hidden");

        if (res !== null) {
            page.Site = res;
            
            if (page.LastPage !== undefined &&
                page.LastPage !== "view-loading")
                page.changeView(page.LastPage);
            else
                page.changeView(page.Site.Pages[0].Name);

            console.log("site", page.Site);
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
            page.changeView(name);

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

    page.login = function () {
        var email = document.querySelector("input[name=email]"),
            emailValid = email.validity.valid,
            pass = document.querySelector("input[name=password]"),
            passValid = pass.validity.valid;

        loginStatus = document.querySelector("section[data-pagename=view-login] .status");

        if (emailValid && passValid) {
            
            page.injectBoundHTML("<paper-spinner active><paper-spinner/>", loginStatus);

            page.$.ajaxLogin.go();
        }
        else {
            if (!emailValid)
                email.focus();
            else if (!passValid)
                pass.focus();

            loginStatus.innerHTML = "";
        }
    };

    page.deletePage = function (id) {
        page.$.ajaxDeletePage.params = '{"id":"' + id + '"}';
        page.$.ajaxDeletePage.go();
        Delete = false;
    };

    page.addPage = function () {
        var firstNavEl = document.querySelector('nav section:nth-child(4)'),
            order;

        if (firstNavEl === null)
            firstNavEl = document.querySelector('nav section:nth-child(3)');

        if (firstNavEl != null)
            order = (parseInt(firstNavEl.getAttribute("data-order"), 10) + 1);
        else order = 0;

        page.$.ajaxAddPage.params = '{"name":"' + page.newPageName + '","order":' + order + '}';

        page.status(
            "page added",
            function () { page.$.ajaxAddPage.go() });

        page.LastPage = undefined;
        page.newPageName = null;
    };

    page.changeView = function(view) {
        page.LastPage = page.SelectedPage;
        page.SelectedPage = view;
        page.setTitle();
    };

    page.setTitle = function () {
        var title = document.querySelector("title");

        page.injectBoundHTML(
            page.SelectedPage.replace(/^view-/, '').replace(/-/g, ' ') + " - Admin",
            title);
    };

    page.home = function () {
        page.changeView(page.LastPage);
    };

    var call = true;
    page.status = function (msg, callback) {
        var status = document.getElementById("status"),
            message = status.querySelector("p"),
            btn = status.querySelector("button");

        page.home();

        message.innerHTML = msg;

        if (callback !== undefined)
            btn.onclick = function () {
                call = false;
                status.setAttribute("hidden", "");
            };
        else
            btn.setAttribute("hidden", "");

        status.removeAttribute("class");

        setTimeout(function () {
            if (call) {
                callback();
                status.setAttribute("hidden", "");
            }
        }, 5000);
    }

    page.toggleNav = function () {
        document.querySelector("core-drawer-panel").togglePanel();
    };

    page.toggleAddOptions = function () {
        if (page.$.addOptions.getAttribute("hidden") == null)
            page.$.addOptions.setAttribute("hidden", "");
        else {
            page.$.addOptions.removeAttribute("hidden");
            page.$.addComponentOptions.setAttribute("hidden", "");
        }
    };

    page.showAddPage = function () {
        page.changeView("view-add-page");
    };

    page.showAddComponentOptions = function () {
        page.$.addOptions.setAttribute("hidden", "");
        page.$.addComponentOptions.removeAttribute("hidden");
    };

    page.showAddComponent = function (e, detail, sender) {
            var elComponent = new ElComponent(),
            name = sender.getAttribute("data-name"),
            component,
            data;

        for (var i in page.Site.Components) {
            if (page.Site.Components[i].Name === name)
                component = page.Site.Components[i];
        }

        elComponent.Name = component.Name;
        elComponent.Props = component.Props;

        page.$.viewComponent.innerHTML = '';
        page.$.viewComponent.appendChild(elComponent);

        page.changeView("view-component");

        page.$.addComponentOptions.setAttribute("hidden", "");
    }
})();
(function () {

    "use strict";

    var page = document.getElementById("page"),
        loginStatus;

    page.SelectedPage = "view-loading";

    function setTitle() {
        var title = document.querySelector("title");
        page.injectBoundHTML(page.SelectedPage.replace(/^view-/, '').replace(/-/g,' ') + " - Admin", title);
    };

    page.ready = function () {
        setTitle();
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
            page.SelectedPage = "view-loading";
        }

        document.querySelector("core-drawer-panel").setAttribute("disableSwipe", "false");
    };

    page.deletePage = function (id) {
        page.$.ajaxDeletePage.params = '{"id":"' + id + '"}';
        page.$.ajaxDeletePage.go();
        page.SelectedPage = "view-loading";
        Delete = false;
    };
    page.addPage = function () {
        var firstNavEl = document.querySelector('nav section:nth-child(4)'),
            order;

        if (firstNavEl === null) {
            firstNavEl = document.querySelector('nav section:nth-child(3)');
        }

        if (firstNavEl != null)
            order = (parseInt(firstNavEl.getAttribute("data-order"), 10) + 1);
        else order = 0;

        page.$.ajaxAddPage.params = '{"name":"' + page.newPageName + '","order":' + order + '}';
        page.$.ajaxAddPage.go();

        page.SelectedPage = "view-loading";

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

    page.showAddComponentOptions = function () {
        page.$.addOptions.setAttribute("hidden", "");

        page.$.addComponentOptions.removeAttribute("hidden");
    };

    page.showAddComponent = function (a, b, e) {
        var elComponent = document.querySelector("section[data-pagename=view-component] el-component"),
            name = e.getAttribute("data-name"),
            component,
            data;

        for (var i in page.Site.Components) {
            if (page.Site.Components[i].Name === name)
                component = page.Site.Components[i];
        }

        data = '{"Type":"' + component.Name + '",';

        for (var i = 0; i < component.Props.length; i++) {
            var prop = component.Props[i];
                
            if (prop.Name != "Type") {
                data += '"' + prop.Name + '":null,';
            }
        }

        data = data.replace(/,$/, '') + "}";
        data = JSON.parse(data)

        elComponent.data = data;
        elComponent.pagename = page.SelectedPage;

        page.SelectedPage = "view-component";

        page.$.addComponentOptions.setAttribute("hidden", "");
    }
})();
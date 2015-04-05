(function () {
    "use strict";

    var self;

    Polymer({
        _id: undefined,
        Name: "loading",
        Props: [],
        pagename: undefined,
        nested: false,

        ready: function () {
            if (this.Name !== undefined) {

                self = this;

                if (this.nested) {
                    this.$.button.setAttribute("hidden", "");

                }
            }
        },
        finish: function () {

            if (this._id == undefined) {
                this.Add();
            }
            else {
                this.Edit();
            }

            this.Props = null;
            this.Name = null;
        },
        close: function () {
            page.SelectedPage = page.LastPage;

            this.Props = null;
            this.Name = null;
        },
        Add: function () {
            var images = [],
                imgI = 0,
                properties = document.querySelector("section[data-pagename=view-component] el-component")
                    .shadowRoot.querySelectorAll("el-property");

            for (var i = 0; i < properties.length; i++) {
                images[imgI] = properties[i].shadowRoot.querySelector("input[type=file]");
                imgI++;
            }

            page.$.ajaxAddComponent.body = new FormData();
            page.$.ajaxAddComponent.contentType = null;

            page.$.ajaxAddComponent.body.append(
                "component",
                JSON.stringify(this.getComponent()).replace(/"/g, "'"));

            page.$.ajaxAddComponent.body.append(
                "componentName",
                this.Name);

            page.$.ajaxAddComponent.body.append(
                "pageName",
                page.LastPage);

            for (var a = 0; a < images.length; a++) {
                if (images[a] != null) {
                    for (var b = 0; b < images[a].files.length; b++) {

                        var file = images[a].files[b];

                        page.$.ajaxAddComponent.body.append(file.name, file);

                    }
                }
            }

            page.$.ajaxAddComponent.go();
        },
        Edit: function () {
            // TODO
        },
        getComponent: function (Props) {

            if (Props === undefined)
                Props = this.Props;

            console.log("props", Props);

            var comp = "{";

            for (var i = 0; i < Props.length; i++) {
                if (Props[i].Type === "Component") {

                    var props = this.$.components.querySelectorAll("el-property"),
                        componentData;

                    for (var a = 0; a < props.length; a++) {
                        if (props[a].Name == Props[i].Name)
                            componentData = props[a].shadowRoot.querySelector("el-component").getComponent();
                    }

                    comp += '"' + Props[i].Name + '":' + JSON.stringify(componentData) + ',';

                }
                else if (Props[i].Type.match(/^List\s*/) != null) {
                    var c = false;

                    comp += '"' + Props[i].Name + '":';

                    console.log("secound get", Props[i]);

                    for (var a = 0; a < page.Site.Components.length; a++) {
                        var component = page.Site.Components[a];

                        if (component.Name === Props[i].Type.replace(/^List\s*/, '')) {
                            c = true;
                        } 
                    }

                    if (!c)
                        comp += JSON.stringify(Props[i].Value) + ",";
                    else {
                        comp += '[';

                        var innerComponents = Props[i].Value;

                        for (var a = 0; a < innerComponents.length; a++) {
                            var innerComponent = JSON.parse(innerComponents[a]);

                            comp += JSON.stringify(this.getComponent(innerComponent.Props)) + ",";
                        }
                        comp = comp.replace(/,$/, "");

                        comp += '],';
                    }
                }
                else {
                    comp += '"' + Props[i].Name + '":"' + Props[i].Value + '",';
                }
            }

            comp = comp.replace(/,$/, "") + "}";

            console.log("get component", comp);

            comp = JSON.parse(comp);

            return comp;
        }
    });

})();
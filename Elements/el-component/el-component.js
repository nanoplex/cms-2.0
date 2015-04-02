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

                console.log("component name", this.Name);
                console.log("component props", this.Props);

                if (this.nested) {
                    this.$.button.setAttribute("hidden", "");

                }
            }
        },
        NameChanged: function () {
            this.ready();
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

            console.log(images);
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
        getComponent: function () {

            var comp = "{";

            for (var i = 0; i < this.Props.length; i++) {
                if (this.Props[i].Type === "Component") {

                    var props = this.$.components.querySelectorAll("el-property"),
                        componentData;

                    for (var a = 0; a < props.length; a++) {
                        if (props[a].Name == this.Props[i].Name)
                            componentData = props[a].shadowRoot.querySelector("el-component").getComponent();
                    }

                    comp += '"' + this.Props[i].Name + '":' + JSON.stringify(componentData) + ',';

                }
                else if (this.Props[i].Type.match(/^List\s*/) != null) {
                    comp += '"' + this.Props[i].Name + '":[';

                    var values = this.Props[i].Value,
                        c = false,
                        type = this.Props[i].Type.replace(/^List /,"");

                    console.log(type);

                    for (var a = 0; a < page.Site.Components.length; a++) {
                        var component = page.Site.Components[a];

                        if (component.Name == type)
                            c = true;
                    }

                    console.log(c);

                    for (var a = 0; a < values.length; a++) {
                        if (c) {
                            comp += '{'
                            for (var b = 0; b < values[a].Props.length; b++) {
                                var p = values[a].Props[b];

                                comp += '"' + p.Name + '":"' + p.Value + '",';
                            }

                            comp = comp.replace(/,$/, '') + '},';

                        } else {
                            comp += '"' + values[a] + '",';
                        }
                        
                    }
                    comp = comp.replace(/,$/, '');

                    comp += '],';
                }
                else {
                    comp += '"' + this.Props[i].Name + '":"' + this.Props[i].Value + '",';
                }

            }

            comp = JSON.parse(comp.replace(/,$/, '') + "}");

            console.log("get component", comp);

            return comp;
        }
    });

})();
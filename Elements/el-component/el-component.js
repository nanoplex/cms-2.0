(function () {
    "use strict";

    Polymer({
        _id: undefined,
        Name: "loading",
        Props: [],
        nested: false,
        ready: function () {
            if (this.nested)
                this.$.button.setAttribute("hidden", "");
        },
        finish: function () {

            var props = this.$.components.querySelectorAll("el-property"),
                inputs = [];

            for (var i = 0; i < props.length; i++) {
                var input = props[i].shadowRoot.querySelector("input");

                if (input !== null)
                    inputs[inputs.length] = input;
                else
                    inputs[inputs.length] = props[i].shadowRoot.querySelector("textarea");
            }

            if (page.validateInputs(inputs)) {
                if (this._id == undefined)
                    this.Add();
                else
                    this.Edit();

                this.Props = null;
                this.Name = null;
            }
        },
        close: function () {
            page.changeView(page.LastPage);

            this.Props = null;
            this.Name = null;
        },
        Add: function () {
            var images = [],
                properties = document.querySelector("section[data-pagename=view-component] el-component")
                    .shadowRoot.querySelectorAll("el-property");

            for (var i = 0; i < properties.length; i++) {
                images[images.length] = properties[i].shadowRoot.querySelector("input[type=file]");
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
            var images = [],
                properties = document.querySelector("section[data-pagename=view-component] el-component")
                    .shadowRoot.querySelectorAll("el-property");

            for (var i = 0; i < properties.length; i++) {
                images[images.length] = properties[i].shadowRoot.querySelector("input[type=file]");
            }

            page.$.ajaxEditComponent.body = new FormData();
            page.$.ajaxEditComponent.contentType = null;

            page.$.ajaxEditComponent.body.append(
                "id",
                this._id);

            page.$.ajaxEditComponent.body.append(
                "component",
                JSON.stringify(this.getComponent()).replace(/"/g, "'"));

            page.$.ajaxEditComponent.body.append(
                "componentName",
                this.Name);

            page.$.ajaxEditComponent.body.append(
                "pageName",
                page.LastPage);

            for (var a = 0; a < images.length; a++) {
                if (images[a] != null) {
                    for (var b = 0; b < images[a].files.length; b++) {
                        var file = images[a].files[b];

                        page.$.ajaxEditComponent.body.append(file.name, file);
                    }
                }
            }

            page.$.ajaxEditComponent.go();
        },
        getComponent: function (Props) {
            if (Props === undefined)
                Props = this.Props;

            var comp = "{";

            for (var i = 0; i < Props.length; i++) {

                if (Props[i].Type.match(/^List\s*/) != null) {
                    comp += this.parseList(Props[i]);
                }
                else {
                    if (page.getComponentByType(Props[i].Type) !== null)
                        comp += this.parseComponent(Props[i]);
                    else
                        comp += '"' + Props[i].Name + '":"' + Props[i].Value + '",';
                }

            }

            comp = comp.replace(/,$/, "") + "}";

            console.log("get component", comp);

            comp = JSON.parse(comp);

            return comp;
        },
        parseComponent: function (Prop) {
            var comp = '',
                props = this.$.components.querySelectorAll("el-property"),
                componentData;

            for (var a = 0; a < props.length; a++) {
                if (props[a].Name == Prop.Name)
                    componentData = props[a].shadowRoot.querySelector("el-component").getComponent();
            }

            return comp + '"' + Prop.Name + '":' + JSON.stringify(componentData) + ',';
        },
        parseList: function (Prop) {
            var comp = '',
                c = false;

            comp += '"' + Prop.Name + '":';

            if (page.getComponentByType(Prop.Type.replace(/^List\s*/, '')) !== null)
                c = true;

            if (!c)
                comp += JSON.stringify(Prop.Value) + ",";
            else {
                comp += '[';

                console.log(Prop.Value);

                for (var a = 0; a < Prop.Value.length; a++) {
                    if (Prop.Value[a] !== undefined) {
                        var data = JSON.parse(Prop.Value[a]);
                        comp += "{";
                        for (var b = 0; b < data.Props.length; b++) {
                            comp += '"' + data.Props[b].Name + '":' + JSON.stringify(data.Props[b].Value) + ',';
                        }
                        comp = comp.replace(/,$/, "") + "},";
                    }
                }

                comp = comp.replace(/,$/, "") + '],';
            }

            return comp;
        }
    });
})();
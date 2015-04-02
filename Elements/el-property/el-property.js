(function () {
    "use strict";

    var testValue = [];

    Polymer({
        Name: "loading",
        Type: undefined,
        Value: undefined,
        ready: function () {
            if (this.isList(this.Type))
                this.Value = [];
        },
        selectedComponent: undefined,
        isList: function (value) {

            if (value.match(/^List/) !== null)
                return true;

            return false;

        },
        listType: function (value) {
            var t = value.replace(/^List\s*/, "");

            if (this.selectedComponent === undefined) {

                for (var i = 0; i < page.Site.Components.length; i++) {
                    var component = page.Site.Components[i];

                    if (component.Name == t)
                        this.selectedComponent = component;
                }
            }

            if (this.selectedComponent !== undefined && t === this.selectedComponent.Name)
                return "Component";

            console.log(":(");
            return t;
        },
        ValueChanged: function () {
            if (this.isList(this.Type)) {
                var items = '';

                console.log("selected component", this.selectedComponent);

                for (var i = 0; i < this.Value.length; i++) {
                    var value = this.Value[i];
                    console.log("value", this.Value);

                    if (value !== undefined) {
                        if (this.selectedComponent !== undefined) {

                            items += "<section layout horizontal justified start>" +
                                     "<article layout vertical>";

                            for (var a = 0; a < this.selectedComponent.Props.length; a++) {
                                items += "<p><b>" + value.Props[a].Name + "</b> " + value.Props[a].Value + "</p>";
                            }

                            items += "</article>" +
                                     "<button data-index='" + i + "' on-tap='{{deleteListItem}}'>" +
                                         "<core-icon icon='close'></core-icon>" +
                                     "</button>" +
                                 "</section>";
                        }
                        else {
                            items += "<section layout horizontal justified>" +
                                          "<p>" + value + "</p>" +
                                          "<button data-index='" + i + "' on-tap='{{deleteListItem}}'>" +
                                              "<core-icon icon='close'></core-icon>" +
                                          "</button>" +
                                      "</section>";
                        }
                    }
                }
                this.injectBoundHTML(items, this.$.listItems);
            }

        },
        showAddListItem: function (e, detail, sender) {
            this.$.add.querySelector("section").setAttribute("hidden", "");

            this.$.add.querySelector("article").removeAttribute("hidden");
        },
        addListItem: function () {
            var value = this.$.add.querySelector("article el-property").Value;

            if (this.Value.length !== undefined)
                this.Value[this.Value.length] = value;
            else this.Value = []; this.Value[0] = value;

            this.$.add.querySelector("section").removeAttribute("hidden");

            this.$.add.querySelector("article").setAttribute("hidden", "");
        },
        deleteListItem: function (e, detail, sender) {
            var i = sender.getAttribute("data-index");

            this.Value[i] = undefined;
        },
        getEmptyComponent: function () {
            if (this.selectedComponent !== undefined)
                return this.selectedComponent;
            else return undefined;
        }
    });
})();
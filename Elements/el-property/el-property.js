(function () {
    "use strict";

    var testValue = [];

    Polymer({
        Name: "loading",
        Type: undefined,
        Value: undefined,
        selectedComponent: undefined,

        ready: function () {
            if (this.isList(this.Type))
                this.Value = [];
        },
        isList: function (value) {
            if (value.match(/^List/) !== null)
                return true;
            return false;
        },
        toJSON: function(value) {
            return JSON.parse(value);
        },
        listType: function (value) {
            var type = value.replace(/^List\s*/, "");

            if (this.selectedComponent === undefined) {
                for (var i = 0; i < page.Site.Components.length; i++) {
                    var component = page.Site.Components[i];

                    if (component.Name == type)
                        this.selectedComponent = component;
                }
            }

            if (this.selectedComponent !== undefined && type === this.selectedComponent.Name)
                return "Component";

            return type;
        },
        showAddListItem: function () {
            this.$.add.querySelector("section").setAttribute("hidden", "");
            this.$.add.querySelector("article").removeAttribute("hidden");
        },
        addListItem: function () {
            var add = this.$.add,
                prop = add.querySelector("article el-property");
            
            if (this.selectedComponent !== undefined) {
                this.Value[this.Value.length] = JSON.stringify(prop.Value);
                prop.Value = '';
            }
            else {
                this.Value[this.Value.length] = prop.Value;
                prop.Value = '';
            }
            
            add.querySelector("section").removeAttribute("hidden");
            add.querySelector("article").setAttribute("hidden", "");
        },
        deleteListItem: function (e, detail, sender) {
            this.Value[parseInt(sender.getAttribute("data-index"), 10)] = undefined;
        },
        getEmptyComponent: function (value) {
            if (this.selectedComponent !== undefined)
                return this.selectedComponent;
            else return value;
        }
    });
})();
(function () {
    "use strict";

    var testValue = [];

    Polymer({
        Name: "loading",
        Type: undefined,
        Value: undefined,
        selectedComponent: undefined,
        ready: function () {
            if (this.isList(this.Type) && this.Value._t !== undefined)
                this.Value = [];
        },
        attached: function () {
            if (this.selectedComponent === undefined) {
                for (var i = 0; i < page.Site.Components.length; i++) {
                    var component = page.Site.Components[i];

                    if (component.Name == this.Type.replace(/^List /, ""))
                        this.selectedComponent = component;
                }
            }
        },
        isList: function (value) {
            if (value.match(/^List/) !== null)
                return true;
            return false;
        },
        toJSON: function (value) {
            if (value._id === undefined)
                return JSON.parse(value);
            else {
                var props = [];

                for (var p in value) {
                    if (p !== "_id")
                        props[props.length] = { "Name": p, "Value": value[p] };
                }
                console.log("new to JSON", props);
                return {"Props": props};
            }
        },
        listType: function (value) {
            return value.replace(/^List\s*/, "");
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
const stylesheetPath = "_content/BlazorSortable/blazor-sortable.css";
let stylesheetLoaded = false;

export function initSortable(id, options, component, loadStylesheet, versionQuery) {
    const el = document.getElementById(id);
    if (!el) {
        return;
    }

    if (loadStylesheet) {
        ensureStylesheet(versionQuery);
    }

    configureGroupCallbacks(options, component);

    el._sortable = new Sortable(el, buildSortableOptions(options, component));
}

export function destroySortable(id) {
    const el = document.getElementById(id);
    if (!el?._sortable) {
        return;
    }

    el._sortable.destroy();
    delete el._sortable;
}

function ensureStylesheet(versionQuery) {
    if (stylesheetLoaded) {
        return;
    }

    const stylesheets = getStylesheets();

    if (stylesheets.some(link => link.href.includes(stylesheetPath))) {
        stylesheetLoaded = true;
        return;
    }

    const link = document.createElement("link");
    link.rel = "stylesheet";
    link.href = stylesheetPath + versionQuery;

    insertStylesheet(stylesheets, link);
    stylesheetLoaded = true;
}

function getStylesheets() {
    return Array.from(document.head.querySelectorAll('link[rel="stylesheet"]'));
}

function insertStylesheet(stylesheets, link) {
    const lastStylesheet = stylesheets[stylesheets.length - 1];

    if (lastStylesheet) {
        lastStylesheet.after(link);
    } else {
        document.head.appendChild(link);
    }
}

function configureGroupCallbacks(options, component) {
    if (options.group.pull === "function") {
        options.group.pull = (to) => component.invokeMethod("OnPullJS", to.el.id);
    }

    if (options.group.put === "function") {
        options.group.put = (_to, from) => component.invokeMethod("OnPutJS", from.el.id);
    }
}

function buildSortableOptions(options, component) {
    return {
        ...options,
        onStart: (evt) => {
            component.invokeMethodAsync("OnStartJS", evt.oldIndex);
        },
        onEnd: () => {
            component.invokeMethodAsync("OnEndJS");
        },
        onUpdate: (evt) => {
            revertDomMove(evt);

            component.invokeMethodAsync("OnUpdateJS", evt.oldIndex, evt.newIndex);
        },
        onAdd: (evt) => {
            component.invokeMethodAsync("OnAddJS", evt.from.id, evt.oldIndex, evt.newIndex, evt.pullMode === "clone");
        },
        onRemove: (evt) => {
            revertDomMove(evt);

            if (evt.pullMode === "clone") {
                evt.clone?.remove();
            } else {
                component.invokeMethodAsync("OnRemoveJS", evt.oldIndex, evt.to.id, evt.newIndex);
            }
        }
    };
}

function revertDomMove(evt) {
    evt.item.remove();
    evt.from.insertBefore(evt.item, evt.from.children[evt.oldIndex]);
}

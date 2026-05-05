const sortableJsPath = "_content/BlazorSortable/Sortable.min.js";
const sortableJsVersion = "1.15.7";

/** @type {Promise<void> | null} */
let sortableJsLoadPromise = null;

const stylesheetPath = "_content/BlazorSortable/blazor-sortable.css";
let stylesheetInjected = false;

export async function initSortable(id, options, component, assetOptions) {
    const el = document.getElementById(id);
    if (!el) {
        return;
    }

    if (assetOptions.loadSortableJs) {
        await ensureSortableJs();
    } else {
        ensureSortableJsAvailable();
    }

    if (assetOptions.loadStylesheet) {
        ensureStylesheet(assetOptions.versionQuery);
    }

    configureGroupCallbacks(options.group, component);

    el.blazorSortable = globalThis.Sortable.create(el, buildSortableOptions(options, component));
}

export function destroySortable(id) {
    const el = document.getElementById(id);
    if (!el?.blazorSortable) {
        return;
    }

    el.blazorSortable.destroy();
    delete el.blazorSortable;
}

async function ensureSortableJs() {
    if (isSortableJsAvailable()) {
        return;
    }

    sortableJsLoadPromise ??= loadSortableJs(`${sortableJsPath}?v=${sortableJsVersion}`);
    await sortableJsLoadPromise;

    ensureSortableJsAvailable();
}

function loadSortableJs(src) {
    return new Promise((resolve, reject) => {
        const sortableJs = document.createElement("script");
        sortableJs.src = src;
        sortableJs.onload = () => resolve();
        sortableJs.onerror = () => {
            sortableJs.remove();
            sortableJsLoadPromise = null;
            reject(new Error(`Failed to load SortableJS from '${src}'.`));
        };

        document.body.appendChild(sortableJs);
    });
}

function ensureSortableJsAvailable() {
    if (isSortableJsAvailable()) {
        return;
    }

    throw new Error(
        "SortableJS is not loaded. Add SortableJS before BlazorSortable initializes, or enable SortableOptions.LoadSortableJs."
    );
}

function isSortableJsAvailable() {
    return typeof globalThis.Sortable === "function";
}

function ensureStylesheet(versionQuery) {
    if (stylesheetInjected) {
        return;
    }

    const stylesheetLinks = getHeadStylesheetLinks();

    if (stylesheetLinks.some(link => link.href.includes(stylesheetPath))) {
        stylesheetInjected = true;
        return;
    }

    const link = document.createElement("link");
    link.rel = "stylesheet";
    link.href = stylesheetPath + versionQuery;

    insertHeadStylesheetLink(stylesheetLinks, link);
    stylesheetInjected = true;
}

function getHeadStylesheetLinks() {
    return Array.from(document.head.querySelectorAll('link[rel="stylesheet"]'));
}

function insertHeadStylesheetLink(stylesheetLinks, link) {
    const lastStylesheetLink = stylesheetLinks[stylesheetLinks.length - 1];

    if (lastStylesheetLink) {
        lastStylesheetLink.after(link);
    } else {
        document.head.appendChild(link);
    }
}

function configureGroupCallbacks(group, component) {
    if (group.pull === "function") {
        group.pull = (to) => component.invokeMethod("OnPullJs", to.el.id);
    }

    if (group.put === "function") {
        group.put = (_to, from) => component.invokeMethod("OnPutJs", from.el.id);
    }
}

function buildSortableOptions(options, component) {
    return {
        ...options,
        onStart: (evt) => {
            component.invokeMethodAsync("OnStartJs", evt.oldIndex);
        },
        onEnd: () => {
            component.invokeMethodAsync("OnEndJs");
        },
        onUpdate: (evt) => {
            revertDomMove(evt);

            component.invokeMethodAsync("OnUpdateJs", evt.oldIndex, evt.newIndex);
        },
        onAdd: (evt) => {
            revertDomMove(evt);

            const isClone = isCloneMode(evt);

            if (isClone) {
                evt.clone.remove();
            }

            component.invokeMethodAsync("OnAddJs", evt.from.id, evt.oldIndex, evt.newIndex, isClone);
        },
        onRemove: (evt) => {
            revertDomMove(evt);

            if (isCloneMode(evt)) {
                evt.clone.remove();
            } else {
                component.invokeMethodAsync("OnRemoveJs", evt.oldIndex, evt.to.id, evt.newIndex);
            }
        }
    };
}

function revertDomMove(evt) {
    const item = evt.item;
    const from = evt.from;
    const oldIndex = evt.oldIndex;

    item.remove();

    const referenceItem = from.children[oldIndex] ?? null;
    from.insertBefore(item, referenceItem);
}

function isCloneMode(evt) {
    return evt.pullMode === "clone";
}

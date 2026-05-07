const sortableJsPath = "_content/BlazorSortable/Sortable.min.js";
const sortableJsVersion = "1.15.7";
let sortableJsLoadPromise = null;

const stylesheetPath = "_content/BlazorSortable/blazor-sortable.css";
let stylesheetInjected = false;

export async function initSortable(id, options, component, assetOptions) {
    const el = document.getElementById(id);
    if (!el) {
        return;
    }

    if (assetOptions.autoLoadSortableJs) {
        await ensureSortableJs();
    } else {
        ensureSortableJsAvailable();
    }

    if (assetOptions.autoLoadStylesheet) {
        ensureStylesheet(assetOptions.versionQuery);
    }

    configureTransferCallbacks(options.group, component);

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

function configureTransferCallbacks(group, component) {
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
            component.invokeMethod(
                "OnStartJs",
                evt.oldIndex,
                getIndexes(evt.oldIndicies));
        },
        onEnd: () => {
            component.invokeMethod("OnEndJs");
        },
        onUpdate: (evt) => {
            const isSwap = Boolean(evt.swapItem);

            revertDomMove(evt, isSwap);

            component.invokeMethodAsync(
                "OnUpdateJs",
                evt.oldIndex,
                getIndexes(evt.oldIndicies),
                evt.newIndex,
                getIndexes(evt.newIndicies),
                isSwap);
        },
        onAdd: (evt) => {
            const isSwap = Boolean(evt.swapItem) && options.swap === true;

            revertDomMove(evt, isSwap);

            const isClone = isCloneMode(evt);

            if (isClone) {
                removeClone(evt);
            }

            component.invokeMethodAsync(
                "OnAddJs",
                evt.from.id,
                evt.oldIndex,
                getIndexes(evt.oldIndicies),
                evt.newIndex,
                getIndexes(evt.newIndicies),
                isClone,
                isSwap);
        },
        onRemove: (evt) => {
            revertDomMove(evt, false);

            const isClone = isCloneMode(evt);

            if (isClone) {
                removeClone(evt);
            }

            component.invokeMethodAsync(
                "OnRemoveJs",
                evt.oldIndex,
                getIndexes(evt.oldIndicies),
                evt.to.id,
                evt.newIndex,
                getIndexes(evt.newIndicies),
                isClone);
        },
        onSelect: (evt) => {
            component.invokeMethodAsync(
                "OnSelectJs",
                getElementIndex(evt.from, evt.item),
                getElementIndexes(evt.from, evt.items));
        },
        onDeselect: (evt) => {
            component.invokeMethodAsync(
                "OnDeselectJs",
                getElementIndex(evt.from, evt.item),
                getElementIndexes(evt.from, evt.items));
        },
        onSpill: (evt) => {
            revertDomMove(evt, false);

            const isClone = isCloneMode(evt);

            if (isClone) {
                removeClone(evt);
            }

            component.invokeMethodAsync(
                "OnSpillJs",
                getElementIndex(evt.from, evt.item),
                getElementIndexes(evt.from, evt.items),
                isClone);
        }
    };
}

function getIndexes(indices) {
    return indices?.map(x => x.index) ?? [];
}

function revertDomMove(evt, isSwap) {
    if (isSwap) {
        revertDomSwap(evt);
        return;
    }

    const pairs = getMovedElementPairs(evt);

    for (const pair of pairs) {
        pair.element.remove();
    }

    for (const pair of pairs) {
        const referenceItem = evt.from.children[pair.oldIndex] ?? null;
        evt.from.insertBefore(pair.element, referenceItem);
    }
}

function revertDomSwap(evt) {
    const pairs = getMovedElementPairs(evt);

    for (const pair of pairs) {
        pair.element.remove();
    }

    evt.swapItem.remove();

    for (const pair of pairs) {
        const referenceItem = evt.from.children[pair.oldIndex] ?? null;
        evt.from.insertBefore(pair.element, referenceItem);
    }

    const swapReferenceItem = evt.to.children[evt.newIndex] ?? null;
    evt.to.insertBefore(evt.swapItem, swapReferenceItem);
}

function getMovedElementPairs(evt) {
    if (evt.oldIndicies?.length > 0) {
        return evt.oldIndicies.map(x => ({
            element: x.multiDragElement,
            oldIndex: x.index
        }));
    }

    return [{
        element: evt.item,
        oldIndex: evt.oldIndex
    }];
}

function isCloneMode(evt) {
    return evt.pullMode === "clone";
}

function removeClone(evt) {
    if (evt.clones?.length > 0) {
        for (const clone of evt.clones) {
            clone.remove();
        }

        return;
    }

    evt.clone?.remove();
}

function getElementIndex(parent, element) {
    return Array.prototype.indexOf.call(parent.children, element);
}

function getElementIndexes(parent, elements) {
    return elements?.map(x => getElementIndex(parent, x)) ?? [];
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
    const firstStylesheetLink = stylesheetLinks[0];

    if (firstStylesheetLink) {
        firstStylesheetLink.before(link);
    } else {
        document.head.appendChild(link);
    }
}

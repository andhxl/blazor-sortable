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
        ensureStylesheet(assetOptions.version);
    }

    configureTransferCallbacks(options.group, component);

    el.blazorSortable = new Sortable(el, buildSortableOptions(options, component));
}

export function destroySortable(id) {
    const el = document.getElementById(id);
    const sortable = el?.blazorSortable;

    if (!sortable) {
        return;
    }

    sortable.destroy();
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
            component.invokeMethodAsync(
                "OnStartJs",
                evt.oldIndex,
                getIndexes(evt.oldIndicies));
        },
        onEnd: () => {
            component.invokeMethodAsync("OnEndJs");
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
            keepSelectionWithinCurrentSortable(evt);

            component.invokeMethodAsync(
                "OnSelectJs",
                getElementIndex(evt.item),
                getElementIndexes(evt.items));
        },
        onDeselect: (evt) => {
            component.invokeMethodAsync(
                "OnDeselectJs",
                getElementIndex(evt.item),
                getElementIndexes(evt.items));
        },
        onSpill: (evt) => {
            revertDomMove(evt, false);

            const isClone = isCloneMode(evt);

            if (isClone) {
                removeClone(evt);
            }

            component.invokeMethodAsync(
                "OnSpillJs",
                getElementIndex(evt.item),
                getElementIndexes(evt.items),
                isClone);
        }
    };
}

function revertDomMove(evt, isSwap) {
    const pairs = getMovedElementPairs(evt);

    for (const pair of pairs) {
        pair.element.remove();
    }

    if (isSwap) {
        evt.swapItem.remove();
    }

    for (const pair of pairs) {
        const referenceItem = evt.from.children[pair.oldIndex] ?? null;
        evt.from.insertBefore(pair.element, referenceItem);
    }

    if (isSwap) {
        const swapReferenceItem = evt.to.children[evt.newIndex] ?? null;
        evt.to.insertBefore(evt.swapItem, swapReferenceItem);
    }
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
    if (evt.clones?.length) {
        for (const clone of evt.clones) {
            clone.remove();
        }

        return;
    }

    evt.clone?.remove();
}

function getIndexes(indices) {
    return indices?.map(x => x.index) ?? [];
}

function keepSelectionWithinCurrentSortable(evt) {
    const currentItems = evt.items.filter(item => item.parentElement === evt.from);

    if (currentItems.length === evt.items.length) {
        return;
    }

    for (const item of evt.items) {
        if (item.parentElement !== evt.from) {
            Sortable.utils.deselect(item);
        }
    }

    evt.items = currentItems;
}

function getElementIndex(element) {
    return Sortable.utils.index(element);
}

function getElementIndexes(elements) {
    return elements?.map(getElementIndex) ?? [];
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

        sortableJs.addEventListener("load", () => resolve(), { once: true });
        sortableJs.addEventListener("error", () => {
            sortableJs.remove();
            sortableJsLoadPromise = null;
            reject(new Error(`Failed to load SortableJS from '${src}'.`));
        }, { once: true });

        document.body.append(sortableJs);
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
    return typeof Sortable === "function";
}

function ensureStylesheet(version) {
    if (stylesheetInjected) {
        return;
    }

    const headStylesheetLinks = getHeadStylesheetLinks();

    if (headStylesheetLinks.some(link => link.href.includes(stylesheetPath))) {
        stylesheetInjected = true;
        return;
    }

    const link = document.createElement("link");
    link.rel = "stylesheet";
    link.href = `${stylesheetPath}?v=${version}`;

    insertHeadStylesheetLink(headStylesheetLinks, link);
    stylesheetInjected = true;
}

function getHeadStylesheetLinks() {
    return Array.from(document.head.querySelectorAll('link[rel="stylesheet"]'));
}

function insertHeadStylesheetLink(headStylesheetLinks, link) {
    const firstHeadStylesheetLink = headStylesheetLinks[0];

    if (firstHeadStylesheetLink) {
        firstHeadStylesheetLink.before(link);
    } else {
        document.head.append(link);
    }
}

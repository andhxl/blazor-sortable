const sortableJsPath = "_content/BlazorSortable/Sortable.min.js";
const sortableJsVersion = "1.15.7";
let sortableJsLoadPromise = null;

const stylesheetPath = "_content/BlazorSortable/blazor-sortable.css";
const highlightStylesheetPath = "_content/BlazorSortable/blazor-sortable-highlights.css";
const injectedStylesheets = new Set();

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
        ensureStylesheet(stylesheetPath, assetOptions.version);

        if (assetOptions.autoLoadHighlightStylesheet) {
            ensureStylesheet(highlightStylesheetPath, assetOptions.version);
        }
    }

    configureTransferCallbacks(options.group, component);

    const sortable = new Sortable(el, buildSortableOptions(options, component));
    el.blazorSortableCleanup = configureTouchAwareMultiDragKey(el, sortable, options);
}

export function destroySortable(id) {
    const el = document.getElementById(id);
    const sortable = el ? Sortable.get(el) : null;

    if (!sortable) {
        return;
    }

    el.blazorSortableCleanup?.();
    delete el.blazorSortableCleanup;

    sortable.destroy();
}

function configureTransferCallbacks(group, component) {
    if (group.pull === "function") {
        group.pull = (to) => component.invokeMethod("OnPullJs", to.el.id);
    }

    if (group.put === "function") {
        group.put = (_to, from) => component.invokeMethod("OnPutJs", from.el.id);
    }
}

function configureTouchAwareMultiDragKey(el, sortable, options) {
    if (!options.multiDragKey) {
        return null;
    }

    const updateMultiDragKey = evt => {
        const isTouchInput = evt.pointerType === "touch" || evt.type === "touchstart";
        sortable.option("multiDragKey", isTouchInput ? "" : options.multiDragKey);
    };

    if (sortable.options.supportPointer) {
        el.addEventListener("pointerdown", updateMultiDragKey, true);
        return () => el.removeEventListener("pointerdown", updateMultiDragKey, true);
    }

    el.addEventListener("mousedown", updateMultiDragKey, true);
    el.addEventListener("touchstart", updateMultiDragKey, true);

    return () => {
        el.removeEventListener("mousedown", updateMultiDragKey, true);
        el.removeEventListener("touchstart", updateMultiDragKey, true);
    };
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
        onEnd: (evt) => {
            component.invokeMethodAsync("OnEndJs");

            resetCrossSortableMultiDragSelection(evt);
        },
        onUpdate: (evt) => {
            const isSwap = Boolean(evt.swapItem);

            revertDomMove(evt, isSwap);

            const newIndex =
                evt.to.children.length === 1 && evt.newIndex === 1
                    ? 0
                    : evt.newIndex;

            component.invokeMethodAsync(
                "OnUpdateJs",
                evt.oldIndex,
                getIndexes(evt.oldIndicies),
                newIndex,
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
    if (evt.oldIndicies?.length) {
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

function resetCrossSortableMultiDragSelection(evt) {
    if (evt.from === evt.to || !evt.items?.length) {
        return;
    }

    setTimeout(() => {
        Sortable.get(evt.to)?.multiDrag?._deselectMultiDrag?.();
    });
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
        "SortableJS is not loaded. Add SortableJS before BlazorSortable initializes, or enable SortableOptions.AutoLoadSortableJs."
    );
}

function isSortableJsAvailable() {
    return typeof Sortable === "function";
}

function ensureStylesheet(path, version) {
    if (injectedStylesheets.has(path)) {
        return;
    }

    const headStylesheetLinks = getHeadStylesheetLinks();

    if (headStylesheetLinks.some(link => link.href.includes(path))) {
        injectedStylesheets.add(path);
        return;
    }

    const link = document.createElement("link");
    link.rel = "stylesheet";
    link.href = `${path}?v=${version}`;

    insertHeadStylesheetLink(headStylesheetLinks, link);
    injectedStylesheets.add(path);
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

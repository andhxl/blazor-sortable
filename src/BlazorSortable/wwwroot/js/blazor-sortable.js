export function initSortable(id, options, component) {
    const el = document.getElementById(id);
    if (!el) return;

    if (options.group.pull === 'function') {
        options.group.pull = function (to, from, dragEl, evt) {
            return component.invokeMethod('OnPullJS', to.el.id);
        };
    }
    if (options.group.put === 'function') {
        options.group.put = function (to, from, dragEl, evt) {
            return component.invokeMethod('OnPutJS', from.el.id);
        };
    }

    el._sortable = new Sortable(el, {
        ...options,
        onStart: (evt) => {
            component.invokeMethodAsync('OnStartJS', evt.oldIndex);
        },
        onEnd: (evt) => {
            component.invokeMethodAsync('OnEndJS');
        },
        onUpdate: (evt) => {
            // Revert the DOM to match the .NET state
            evt.item.remove();
            evt.from.insertBefore(evt.item, evt.from.children[evt.oldIndex]);

            // Notify .NET to update its model and re-render
            component.invokeMethodAsync('OnUpdateJS', evt.oldIndex, evt.newIndex);
        },
        onAdd: (evt) => {
            component.invokeMethodAsync('OnAddJS', evt.from.id, evt.oldIndex, evt.newIndex, evt.pullMode === 'clone');
        },
        onRemove: (evt) => {
            evt.item.remove();
            evt.from.insertBefore(evt.item, evt.from.children[evt.oldIndex]);

            if (evt.pullMode === 'clone') {
                evt.clone?.remove(); // Handle MultiDrag null case
            }
            else {
                component.invokeMethodAsync('OnRemoveJS', evt.oldIndex, evt.to.id, evt.newIndex);
            }
        }
    });
}

export function destroySortable(id) {
    const el = document.getElementById(id);
    if (!el) return;

    if (el._sortable) {
        el._sortable.destroy();
        delete el._sortable;
    }
}

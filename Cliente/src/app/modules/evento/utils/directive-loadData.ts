import { Directive, ElementRef, Output, EventEmitter, Input } from '@angular/core';
@Directive({
    selector: '[loadData]',
})
export class DirectiveLoadData {
    constructor(private element: ElementRef) { }
    @Output('elementVisible') elementVisible = new EventEmitter<boolean>();
    @Input('isTargetElement') isTargetElement: boolean;

    public intersectionOptions = {
        root: null,
        rootMargin: '0px',
        threshold: [0, 0.5, 1],
    };

    ngAfterViewInit() {
        let observer = new IntersectionObserver(
            this.intersectionCallback.bind(this),
            this.intersectionOptions
        );
        if (this.isTargetElement) {
            observer.observe(this.element.nativeElement);
        }
    }

    intersectionCallback(entries:any, observer:any) {
        entries.forEach((entry:any) => {
            if (entry.intersectionRatio === 1) {
                this.elementVisible.emit(true);
            } else {
                this.elementVisible.emit(false);
            }
        });
    }
}

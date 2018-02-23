import { Component, ContentChild, ElementRef, Input, NgZone, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { BehaviorSubject } from "rxjs/BehaviorSubject";
import { NgbDropdown } from "@ng-bootstrap/ng-bootstrap";

@Component({
    selector: "app-dropdown-box",
    templateUrl: "dropdown-box.component.html",
    styleUrls: ["dropdown-box.component.css"]
})
export class DropdownBoxComponent implements OnInit {
    @ViewChild(NgbDropdown)
    private container: NgbDropdown;

    @ViewChild("button")
    private button: ElementRef;

    @ContentChild(TemplateRef)
    template: TemplateRef<any>;

    @Input()
    tooltip = "";

    isOpen$ = new BehaviorSubject(false);

    constructor(private zone: NgZone) {
    }

    ngOnInit(): void {
        //const container = $(this.container.nativeElement);
        //container.on("show.bs.dropdown", () => this.zone.run(() => this.isOpen$.next(true)));
        //container.on("hide.bs.dropdown", () => this.zone.run(() => this.isOpen$.next(false)));
    }

    toggle(): void {
        this.container.close();
    }
}

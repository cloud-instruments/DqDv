import { Component, Input, OnInit, EventEmitter  } from "@angular/core";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";

@Component({
    selector: "app-confirm",
    templateUrl: "confirm-popup.component.html",
})
export class ConfirmPopupComponent implements OnInit {
    @Input() title: string = "Modal with component";
    @Input() message: string = "Message here...";
    onHide: EventEmitter<boolean>;

    constructor(public activeModal: NgbActiveModal) { }

    public ngOnInit(): void {
        this.onHide = new EventEmitter<boolean>();
    }

    public Ok() {
        this.onHide.emit(true);
        this.activeModal.close(true);
    }

    public Cancel() {
        this.activeModal.close(false);
    }
}
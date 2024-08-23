import { ColumnDefinition, RowComponent, Sorter, TabulatorFull } from "tabulator-tables";
import { Observable, Subscription } from "rxjs";
import {
  AfterViewInit,
  ChangeDetectionStrategy,
  Component,
  DestroyRef,
  ElementRef,
  EventEmitter,
  inject,
  Input,
  OnDestroy,
  OnInit,
  Output,
  signal,
  ViewChild
} from "@angular/core";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";

@Component({
  selector: "tabulator-basic",
  standalone: true,
  templateUrl: "./tabulator-basic.component.html",
  styleUrl: "./tabulator-basic.component.scss",
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TabulatorBasicComponent implements AfterViewInit, OnInit, OnDestroy {
  @Input() initialSort: Sorter[] = [];
  @Input({ required: true }) ajaxUrl: string | undefined;
  @Input({ required: true }) columnDefinitions: ColumnDefinition[] = [];
  @Input() paginationSizeSelector: number[] = [15, 25, 35, 50];
  @Input() paginationElement?: HTMLElement | undefined;
  @Input() tableHeight: string = "Auto";
  @Input({ required: true }) dataChangedExternally$: Observable<boolean> | undefined;
  @Input({ required: true }) ajaxRequestOutgoing$: Observable<boolean> | undefined;
  @Output() ajaxDataInComing = new EventEmitter<boolean>();
  @Output() selectedCellData = new EventEmitter<any[]>();
  @Output() ajaxErrorResponse = new EventEmitter<Error>();
  private readonly $ajaxRequestOutgoing = signal(false);
  private dataChanged: Subscription | undefined;
  private readonly destroyRef = inject(DestroyRef);

  @ViewChild("tabulatorTable", { read: ElementRef })
  tabulatorTable: ElementRef<HTMLElement> | undefined;

  ngOnInit(): void {
    if (this.ajaxRequestOutgoing$ != undefined) {
      this.ajaxRequestOutgoing$.pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
        next: (val) => {
          this.$ajaxRequestOutgoing.set(val);
        }
      });
    }
  }

  ngAfterViewInit(): void {
    this.drawTable();
  }

  ngOnDestroy(): void {
    this.destroySubscriptions();
  }

  emitSelectedCellData(val: any[]) {
    this.selectedCellData.emit(val);
  }

  emitAjaxDataInComingEvent(val: boolean) {
    this.ajaxDataInComing.emit(val);
  }

  emitAjaxResponse(err: Error) {
    this.ajaxErrorResponse.emit(err);
  }

  private destroySubscriptions() {
    if (this.dataChanged) {
      this.dataChanged.unsubscribe();
    }
  }

  private drawTable(): void {
    if (!this.tabulatorTable) {
      return;
    }

    const table = new TabulatorFull(this.tabulatorTable.nativeElement, {
      placeholder: "No Data Available",
      selectableRows: true,
      rowHeader:
      {
        resizable: true, frozen: true, headerHozAlign: "center", hozAlign: "center", formatter: "rowSelection", titleFormatter: "rowSelection", cellClick: function (e, cell) {
          cell.getRow().toggleSelect();
        }
      },
      paginationSizeSelector: this.paginationSizeSelector,
      columns: this.columnDefinitions,
      height: this.tableHeight,
      ajaxURL: this.ajaxUrl,
      pagination: true,
      paginationMode: "remote",
      filterMode: "remote",
      sortMode: "remote",
      paginationCounter: "rows",
      paginationSize: 15,
      ajaxContentType: "json",
      layout: "fitDataStretch",
      initialSort: this.initialSort,
      ajaxConfig: {
        method: "POST",
        mode: "cors",
        credentials: "include",
        headers: {
          Accept: "application/json",
          "X-Requested-With": "XMLHttpRequest",
          "Content-type": "application/json; charset=utf-8"
        }
      }
    });

    table.on("tableBuilt", () => {
      if (this.dataChangedExternally$ != undefined) {
        this.dataChanged = this.dataChangedExternally$.subscribe(async (val: boolean) => {
          if (val) {
            await table.setData();
          }
        });
      }
    });

    table.on("tableDestroyed", () => {
      this.destroySubscriptions();
    });

    table.on("dataLoading", () => {
      this.emitAjaxDataInComingEvent(true);
    });

    table.on("dataLoaded", () => {
      this.emitAjaxDataInComingEvent(false);
    });

    table.on("dataLoadError", (err: Error) => {
      this.emitAjaxResponse(err);
      table.setHeight(200);
      this.emitAjaxDataInComingEvent(false);
    });

    table.on("rowSelected", (row: RowComponent) => {
      if (!this.$ajaxRequestOutgoing()) {
        this.emitSelectedCellData(table.getSelectedData());
      }
    });

    table.on("rowDeselected", () => {
      if (!this.$ajaxRequestOutgoing()) {
        this.emitSelectedCellData(table.getSelectedData());
      }
    });
  }
}

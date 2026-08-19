from __future__ import annotations

from pathlib import Path
from reportlab.lib import colors
from reportlab.lib.enums import TA_CENTER, TA_LEFT
from reportlab.lib.pagesizes import A4
from reportlab.lib.styles import ParagraphStyle, getSampleStyleSheet
from reportlab.lib.units import mm
from reportlab.platypus import (
    BaseDocTemplate,
    Frame,
    PageTemplate,
    Paragraph,
    Spacer,
    Table,
    TableStyle,
    PageBreak,
)
from reportlab.pdfgen import canvas


ROOT = Path(r"D:\Acedmy")
OUT_DIR = ROOT / "output" / "pdf"
TMP_DIR = ROOT / "tmp" / "pdfs"
OUT_DIR.mkdir(parents=True, exist_ok=True)
TMP_DIR.mkdir(parents=True, exist_ok=True)

OUTPUT_PDF = OUT_DIR / "api_authorization_guide.pdf"

GREEN = colors.HexColor("#2E7D32")
DARK_GREEN = colors.HexColor("#1B5E20")
LIGHT_GREEN = colors.HexColor("#E8F5E9")
MINT = colors.HexColor("#F5FBF6")
TEXT = colors.HexColor("#1A1A1A")
MUTED = colors.HexColor("#5F6F61")
LINE = colors.HexColor("#D7E4D8")


def build_pdf(path: Path) -> None:
    styles = getSampleStyleSheet()
    styles.add(
        ParagraphStyle(
            name="HeroTitle",
            parent=styles["Title"],
            fontName="Helvetica-Bold",
            fontSize=24,
            leading=30,
            textColor=TEXT,
            alignment=TA_LEFT,
            spaceAfter=8,
        )
    )
    styles.add(
        ParagraphStyle(
            name="HeroSubtitle",
            parent=styles["BodyText"],
            fontName="Helvetica",
            fontSize=11,
            leading=16,
            textColor=MUTED,
            spaceAfter=10,
        )
    )
    styles.add(
        ParagraphStyle(
            name="SectionTitle",
            parent=styles["Heading1"],
            fontName="Helvetica-Bold",
            fontSize=18,
            leading=22,
            textColor=DARK_GREEN,
            spaceBefore=6,
            spaceAfter=10,
        )
    )
    styles.add(
        ParagraphStyle(
            name="SubTitle",
            parent=styles["Heading2"],
            fontName="Helvetica-Bold",
            fontSize=12,
            leading=15,
            textColor=TEXT,
            spaceBefore=4,
            spaceAfter=6,
        )
    )
    styles.add(
        ParagraphStyle(
            name="Body",
            parent=styles["BodyText"],
            fontName="Helvetica",
            fontSize=10.5,
            leading=15,
            textColor=TEXT,
            spaceAfter=7,
        )
    )
    styles.add(
        ParagraphStyle(
            name="BodyMuted",
            parent=styles["BodyText"],
            fontName="Helvetica",
            fontSize=10,
            leading=14,
            textColor=MUTED,
            spaceAfter=6,
        )
    )
    styles.add(
        ParagraphStyle(
            name="SmallLabel",
            parent=styles["BodyText"],
            fontName="Helvetica-Bold",
            fontSize=8.5,
            leading=10,
            textColor=DARK_GREEN,
            alignment=TA_CENTER,
        )
    )

    page_width, page_height = A4
    doc = BaseDocTemplate(
        str(path),
        pagesize=A4,
        leftMargin=18 * mm,
        rightMargin=18 * mm,
        topMargin=18 * mm,
        bottomMargin=16 * mm,
    )

    frame = Frame(
        doc.leftMargin,
        doc.bottomMargin,
        page_width - doc.leftMargin - doc.rightMargin,
        page_height - doc.topMargin - doc.bottomMargin,
        id="normal",
    )
    doc.addPageTemplates(
        [
            PageTemplate(id="main", frames=[frame], onPage=make_on_page(styles)),
        ]
    )

    story = []
    story.extend(build_cover(styles))
    story.append(PageBreak())
    story.extend(build_pages(styles))
    doc.build(story)


def make_on_page(styles):
    def on_page(canv: canvas.Canvas, doc):
        page_num = canv.getPageNumber()
        width, height = A4

        canv.saveState()
        canv.setFillColor(colors.white)
        canv.rect(0, 0, width, height, fill=1, stroke=0)

        canv.setFillColor(MINT)
        canv.rect(0, height - 16 * mm, width, 16 * mm, fill=1, stroke=0)
        canv.setFillColor(GREEN)
        canv.rect(0, height - 4 * mm, width, 4 * mm, fill=1, stroke=0)

        canv.setFillColor(DARK_GREEN)
        canv.setFont("Helvetica-Bold", 9)
        canv.drawString(doc.leftMargin, height - 10.5 * mm, "API Authorization Made Simple")

        canv.setFillColor(MUTED)
        canv.setFont("Helvetica", 8)
        canv.drawRightString(width - doc.rightMargin, height - 10.5 * mm, "General educational guide")

        canv.setStrokeColor(LINE)
        canv.setLineWidth(0.8)
        canv.line(doc.leftMargin, 14 * mm, width - doc.rightMargin, 14 * mm)

        canv.setFillColor(MUTED)
        canv.setFont("Helvetica", 8)
        canv.drawString(doc.leftMargin, 8 * mm, "Teach the concept, not the framework.")
        canv.drawRightString(width - doc.rightMargin, 8 * mm, f"Page {page_num}")
        canv.restoreState()

    return on_page


def card_table(rows, col_widths, bg=colors.white, padding=10):
    table = Table(rows, colWidths=col_widths)
    table.setStyle(
        TableStyle(
            [
                ("BACKGROUND", (0, 0), (-1, -1), bg),
                ("BOX", (0, 0), (-1, -1), 0.8, LINE),
                ("INNERGRID", (0, 0), (-1, -1), 0.5, LINE),
                ("LEFTPADDING", (0, 0), (-1, -1), padding),
                ("RIGHTPADDING", (0, 0), (-1, -1), padding),
                ("TOPPADDING", (0, 0), (-1, -1), padding),
                ("BOTTOMPADDING", (0, 0), (-1, -1), padding),
                ("VALIGN", (0, 0), (-1, -1), "TOP"),
            ]
        )
    )
    return table


def build_cover(styles):
    story = []
    story.append(Spacer(1, 10 * mm))

    hero = card_table(
        [
            [
                Paragraph("<font color='#2E7D32'><b>EDUCATIONAL GUIDE / PDF</b></font>", styles["SmallLabel"]),
                "",
            ],
            [
                Paragraph(
                    "API Authorization Made Simple: A Clear Guide to Permission-Based Access Control",
                    styles["HeroTitle"],
                ),
                Paragraph(
                    "A polished, beginner-friendly explainer that breaks down how authentication, "
                    "authorization, policies, and permission checks work together in a backend system.",
                    styles["HeroSubtitle"],
                ),
            ],
        ],
        [95 * mm, 75 * mm],
        bg=colors.white,
        padding=12,
    )
    hero.setStyle(
        TableStyle(
            [
                ("SPAN", (0, 0), (1, 0)),
                ("SPAN", (0, 1), (0, 1)),
                ("SPAN", (1, 1), (1, 1)),
                ("BACKGROUND", (0, 0), (-1, -1), colors.white),
                ("BOX", (0, 0), (-1, -1), 1.2, LINE),
                ("ROUNDEDCORNERS", [8, 8, 8, 8]),
            ]
        )
    )
    story.append(hero)
    story.append(Spacer(1, 8 * mm))

    intro = card_table(
        [
            [
                Paragraph("<b>Why this matters</b>", styles["SubTitle"]),
                Paragraph(
                    "Every modern API needs more than login. It needs a way to decide who can do what. "
                    "That is where authorization comes in. This guide helps your audience understand the idea in plain language.",
                    styles["Body"],
                ),
            ],
            [
                Paragraph("<b>What readers will learn</b>", styles["SubTitle"]),
                Paragraph(
                    "The difference between authentication and authorization, the request flow behind permission checks, "
                    "and a simple mental model they can reuse in any backend stack.",
                    styles["Body"],
                ),
            ],
        ],
        [44 * mm, 126 * mm],
        bg=MINT,
    )
    intro.setStyle(TableStyle([("SPAN", (0, 0), (1, 0)), ("SPAN", (0, 1), (1, 1))]))
    story.append(intro)
    story.append(Spacer(1, 7 * mm))

    highlights = card_table(
        [
            [
                Paragraph("<b>Three key ideas</b>", styles["SubTitle"]),
                Paragraph(
                    "1. Authentication proves who the user is.<br/>"
                    "2. Authorization proves what the user can do.<br/>"
                    "3. A clean policy layer keeps both concerns separate.",
                    styles["Body"],
                ),
            ]
        ],
        [42 * mm, 128 * mm],
        bg=colors.white,
    )
    highlights.setStyle(TableStyle([("SPAN", (0, 0), (1, 0))]))
    story.append(highlights)
    return story


def build_pages(styles):
    story = []

    story.append(Paragraph("1. The Core Idea", styles["SectionTitle"]))
    story.append(
        Paragraph(
            "Authorization is the security checkpoint that runs after login. "
            "The user may already be authenticated, but the API still needs to decide whether that person may create, view, update, or delete a resource.",
            styles["Body"],
        )
    )

    flow = Table(
        [[
            Paragraph("<b>Request</b>", styles["SmallLabel"]),
            Paragraph("<b>JWT Auth</b>", styles["SmallLabel"]),
            Paragraph("<b>Permission Policy</b>", styles["SmallLabel"]),
            Paragraph("<b>Handler</b>", styles["SmallLabel"]),
            Paragraph("<b>Response</b>", styles["SmallLabel"]),
        ]],
        colWidths=[30 * mm, 32 * mm, 42 * mm, 28 * mm, 28 * mm],
    )
    flow.setStyle(
        TableStyle(
            [
                ("BACKGROUND", (0, 0), (-1, -1), LIGHT_GREEN),
                ("TEXTCOLOR", (0, 0), (-1, -1), DARK_GREEN),
                ("BOX", (0, 0), (-1, -1), 1, LINE),
                ("INNERGRID", (0, 0), (-1, -1), 0.7, LINE),
                ("ALIGN", (0, 0), (-1, -1), "CENTER"),
                ("VALIGN", (0, 0), (-1, -1), "MIDDLE"),
                ("TOPPADDING", (0, 0), (-1, -1), 10),
                ("BOTTOMPADDING", (0, 0), (-1, -1), 10),
            ]
        )
    )
    story.append(Spacer(1, 5 * mm))
    story.append(flow)
    story.append(Spacer(1, 6 * mm))

    story.append(Paragraph("2. Authentication vs Authorization", styles["SectionTitle"]))
    comparison = card_table(
        [
            [
                Paragraph("<b>Authentication</b>", styles["SubTitle"]),
                Paragraph("<b>Authorization</b>", styles["SubTitle"]),
            ],
            [
                Paragraph(
                    "Confirms identity.<br/>Example: username, password, token, or session.",
                    styles["Body"],
                ),
                Paragraph(
                    "Confirms access rights.<br/>Example: can this user call a specific endpoint?",
                    styles["Body"],
                ),
            ],
        ],
        [88 * mm, 88 * mm],
        bg=colors.white,
    )
    story.append(comparison)
    story.append(Spacer(1, 5 * mm))

    story.append(Paragraph("3. How a Permission Check Usually Works", styles["SectionTitle"]))
    story.append(
        Paragraph(
            "A typical API permission check uses a policy name such as <b>Permission:academy.create</b>. "
            "The framework translates that label into a rule, reads the JWT claims, loads the user from the database, "
            "and asks whether the user's role contains the required permission.",
            styles["Body"],
        )
    )

    steps = [
        "The client sends the request with a bearer token.",
        "The API validates the token and extracts the user identity.",
        "A permission requirement is created from the endpoint label.",
        "The authorization handler checks the database for that role's permissions.",
        "The result handler returns a friendly 401 or 403 message when access is denied.",
    ]
    step_rows = []
    for idx, text in enumerate(steps, start=1):
        step_rows.append(
            [
                Paragraph(f"<b>{idx}</b>", styles["SmallLabel"]),
                Paragraph(text, styles["Body"]),
            ]
        )
    steps_table = Table(step_rows, colWidths=[12 * mm, 162 * mm])
    steps_table.setStyle(
        TableStyle(
            [
                ("BACKGROUND", (0, 0), (-1, -1), colors.white),
                ("BOX", (0, 0), (-1, -1), 0.8, LINE),
                ("INNERGRID", (0, 0), (-1, -1), 0.5, LINE),
                ("VALIGN", (0, 0), (-1, -1), "TOP"),
                ("LEFTPADDING", (0, 0), (-1, -1), 8),
                ("RIGHTPADDING", (0, 0), (-1, -1), 8),
                ("TOPPADDING", (0, 0), (-1, -1), 8),
                ("BOTTOMPADDING", (0, 0), (-1, -1), 8),
            ]
        )
    )
    story.append(steps_table)
    story.append(PageBreak())

    story.append(Paragraph("4. Why Authorization Deserves Its Own Layer", styles["SectionTitle"]))
    story.append(
        Paragraph(
            "Authorization is not business logic. It is a cross-cutting concern. "
            "That is why it lives in a dedicated folder: it protects multiple modules without forcing every handler to repeat the same checks.",
            styles["Body"],
        )
    )

    reasons = card_table(
        [
            [
                Paragraph("<b>Cleaner code</b>", styles["SubTitle"]),
                Paragraph("You keep security rules out of every handler.", styles["Body"]),
            ],
            [
                Paragraph("<b>Reusable policies</b>", styles["SubTitle"]),
                Paragraph("One permission model can protect many endpoints.", styles["Body"]),
            ],
            [
                Paragraph("<b>Better error output</b>", styles["SubTitle"]),
                Paragraph("Users get consistent 401 and 403 responses.", styles["Body"]),
            ],
            [
                Paragraph("<b>Easy to extend</b>", styles["SubTitle"]),
                Paragraph("You can add a new permission without rewriting the pipeline.", styles["Body"]),
            ],
        ],
        [50 * mm, 118 * mm],
        bg=colors.white,
    )
    story.append(reasons)
    story.append(Spacer(1, 6 * mm))

    story.append(Paragraph("5. Common Mistakes to Avoid", styles["SectionTitle"]))
    mistakes = [
        "Checking permissions only in the frontend.",
        "Treating login as the same thing as authorization.",
        "Returning a blank 403 with no explanation.",
        "Hardcoding access rules inside every controller action.",
        "Forgetting to separate module access from endpoint access.",
    ]
    mistake_rows = [[Paragraph(f"- {m}", styles["Body"])] for m in mistakes]
    mistake_table = Table(mistake_rows, colWidths=[176 * mm])
    mistake_table.setStyle(
        TableStyle(
            [
                ("BACKGROUND", (0, 0), (-1, -1), MINT),
                ("BOX", (0, 0), (-1, -1), 0.8, LINE),
                ("LEFTPADDING", (0, 0), (-1, -1), 10),
                ("RIGHTPADDING", (0, 0), (-1, -1), 10),
                ("TOPPADDING", (0, 0), (-1, -1), 8),
                ("BOTTOMPADDING", (0, 0), (-1, -1), 8),
            ]
        )
    )
    story.append(mistake_table)
    story.append(Spacer(1, 6 * mm))

    story.append(Paragraph("6. A Simple Way to Explain It", styles["SectionTitle"]))
    story.append(
        Paragraph(
            "<b>Short version:</b> authentication says who you are, authorization says what you are allowed to do, "
            "and a policy-based handler decides whether each API request should be accepted or rejected.",
            styles["Body"],
        )
    )
    story.append(
        Paragraph(
            "That sentence is short enough to remember and clear enough for a beginner to reuse.",
            styles["BodyMuted"],
        )
    )

    outro = card_table(
        [
            [
                Paragraph("<b>Closing note</b>", styles["SubTitle"]),
                Paragraph(
                    "Secure APIs are not built by adding more code. They are built by separating concerns, "
                    "making permissions explicit, and returning understandable responses when access is denied.",
                    styles["Body"],
                ),
            ]
        ],
        [52 * mm, 116 * mm],
        bg=LIGHT_GREEN,
    )
    story.append(outro)
    return story


if __name__ == "__main__":
    build_pdf(OUTPUT_PDF)
    print(str(OUTPUT_PDF))

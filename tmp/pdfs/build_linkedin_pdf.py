from reportlab.lib import colors
from reportlab.lib.enums import TA_CENTER, TA_LEFT
from reportlab.lib.pagesizes import A4
from reportlab.lib.styles import ParagraphStyle, getSampleStyleSheet
from reportlab.lib.units import mm
from reportlab.platypus import (
    BaseDocTemplate, Frame, PageTemplate, Paragraph, Spacer, PageBreak,
    Table, TableStyle, KeepTogether
)
from reportlab.pdfbase.ttfonts import TTFont
from reportlab.pdfbase import pdfmetrics
from reportlab.lib.colors import HexColor
from reportlab.lib.utils import simpleSplit
from pathlib import Path

OUT = Path('D:/Acedmy/output/pdf/ai-future-software-development.pdf')
OUT.parent.mkdir(parents=True, exist_ok=True)

NAVY = HexColor('#10243E')
BLUE = HexColor('#2F6FED')
SKY = HexColor('#EAF2FF')
VIOLET = HexColor('#6E5AE6')
INK = HexColor('#172033')
MUTED = HexColor('#5F6B7A')
PALE = HexColor('#F6F8FC')
LINE = HexColor('#DCE4EF')
WHITE = colors.white
RED = HexColor('#B64747')

font_regular = 'Helvetica'
font_bold = 'Helvetica-Bold'

styles = getSampleStyleSheet()
styles.add(ParagraphStyle('Kicker', fontName=font_bold, fontSize=9, leading=12, textColor=BLUE, tracking=1.3, spaceAfter=7))
styles.add(ParagraphStyle('TitleHero', fontName=font_bold, fontSize=31, leading=35, textColor=NAVY, spaceAfter=14))
styles.add(ParagraphStyle('SubtitleHero', fontName=font_regular, fontSize=15, leading=22, textColor=MUTED, spaceAfter=18))
styles.add(ParagraphStyle('PageTitle', fontName=font_bold, fontSize=25, leading=29, textColor=NAVY, spaceAfter=10))
styles.add(ParagraphStyle('SectionTitle', fontName=font_bold, fontSize=18, leading=23, textColor=NAVY, spaceAfter=8))
styles.add(ParagraphStyle('Body', fontName=font_regular, fontSize=10.8, leading=17, textColor=INK, spaceAfter=9))
styles.add(ParagraphStyle('BodySmall', fontName=font_regular, fontSize=9.4, leading=14, textColor=INK, spaceAfter=5))
styles.add(ParagraphStyle('Quote', fontName=font_bold, fontSize=18, leading=24, textColor=NAVY, alignment=TA_CENTER, spaceAfter=6))
styles.add(ParagraphStyle('QuoteSmall', fontName=font_bold, fontSize=12.5, leading=17, textColor=NAVY, alignment=TA_LEFT))
styles.add(ParagraphStyle('WhiteQuote', fontName=font_bold, fontSize=18, leading=24, textColor=WHITE, alignment=TA_CENTER))
styles.add(ParagraphStyle('WhiteBodySmall', fontName=font_regular, fontSize=9.4, leading=14, textColor=WHITE, alignment=TA_CENTER))
styles.add(ParagraphStyle('CardTitle', fontName=font_bold, fontSize=11, leading=14, textColor=NAVY, spaceAfter=5))
styles.add(ParagraphStyle('CardBody', fontName=font_regular, fontSize=9.2, leading=13, textColor=MUTED))
styles.add(ParagraphStyle('Footer', fontName=font_regular, fontSize=8, leading=10, textColor=MUTED))

def P(text, style='Body'):
    return Paragraph(text, styles[style])

def bullets(items, style='BodySmall'):
    return [P('&bull; ' + item, style) for item in items]

def card(title, body, fill=WHITE, accent=BLUE):
    data = [[P(title, 'CardTitle')], [P(body, 'CardBody')]]
    t = Table(data, colWidths=[82*mm], hAlign='LEFT')
    t.setStyle(TableStyle([
        ('BACKGROUND', (0, 0), (-1, -1), fill),
        ('BOX', (0, 0), (-1, -1), 0.7, LINE),
        ('LINEBEFORE', (0, 0), (0, -1), 4, accent),
        ('LEFTPADDING', (0, 0), (-1, -1), 12),
        ('RIGHTPADDING', (0, 0), (-1, -1), 12),
        ('TOPPADDING', (0, 0), (-1, 0), 11),
        ('BOTTOMPADDING', (0, -1), (-1, -1), 11),
    ]))
    return t

def two_cards(items, fill=WHITE):
    row = [card(title, body, fill=fill, accent=BLUE if i % 2 == 0 else VIOLET) for i, (title, body) in enumerate(items)]
    t = Table([row], colWidths=[88*mm, 88*mm], hAlign='LEFT')
    t.setStyle(TableStyle([('VALIGN', (0, 0), (-1, -1), 'TOP'), ('LEFTPADDING', (0, 0), (-1, -1), 0), ('RIGHTPADDING', (0, 0), (-1, -1), 7)]))
    return t

def page_header(canvas, doc):
    canvas.saveState()
    w, h = A4
    canvas.setStrokeColor(LINE)
    canvas.setLineWidth(0.6)
    canvas.line(18*mm, h-17*mm, w-18*mm, h-17*mm)
    canvas.setFont(font_bold, 8)
    canvas.setFillColor(NAVY)
    canvas.drawString(18*mm, h-13*mm, 'AI AND THE FUTURE OF SOFTWARE DEVELOPMENT')
    canvas.setFont(font_regular, 8)
    canvas.setFillColor(MUTED)
    canvas.drawRightString(w-18*mm, h-13*mm, 'A practical reflection')
    canvas.line(18*mm, 17*mm, w-18*mm, 17*mm)
    canvas.setFont(font_regular, 8)
    canvas.drawString(18*mm, 11*mm, 'Building, learning, and thinking with AI')
    canvas.drawRightString(w-18*mm, 11*mm, f'{doc.page}')
    canvas.restoreState()

class StoryDoc(BaseDocTemplate):
    def __init__(self, filename, **kwargs):
        super().__init__(filename, pagesize=A4, leftMargin=18*mm, rightMargin=18*mm, topMargin=25*mm, bottomMargin=23*mm, **kwargs)
        frame = Frame(self.leftMargin, self.bottomMargin, self.width, self.height, id='normal')
        self.addPageTemplates([PageTemplate(id='story', frames=frame, onPage=page_header)])

story = []

# Cover
story += [Spacer(1, 24*mm), P('A PRACTICAL EXPERIENCE WITH AI', 'Kicker'), P('AI and the Future of<br/>Software Development', 'TitleHero'), P('Is AI replacing software developers, or is it changing who can build software?', 'SubtitleHero'), Spacer(1, 11*mm)]
cover = Table([[P('I had limited practical experience with React and JavaScript. With AI assistance, I built and connected a working frontend application.', 'Quote')]], colWidths=[170*mm])
cover.setStyle(TableStyle([('BACKGROUND', (0,0), (-1,-1), SKY), ('BOX', (0,0), (-1,-1), 0.8, HexColor('#BFD4FA')), ('LEFTPADDING', (0,0), (-1,-1), 18), ('RIGHTPADDING', (0,0), (-1,-1), 18), ('TOPPADDING', (0,0), (-1,-1), 20), ('BOTTOMPADDING', (0,0), (-1,-1), 20)]))
story += [cover, Spacer(1, 17*mm), P('A personal reflection on AI-assisted development, the new accessibility of software creation, and the real risks for developers.', 'Body'), Spacer(1, 30*mm), P('By a backend developer learning while building', 'BodySmall'), PageBreak()]

# 2
story += [P('01 / THE BIG QUESTION', 'Kicker'), P('Is AI replacing developers?', 'PageTitle'), P('This is no longer a theoretical discussion. AI can generate code, explain unfamiliar technologies, suggest the next step, identify common errors, create testing guidance, design interfaces, and connect applications to APIs.', 'Body'), P('The more important question may be this:', 'Body'), P('Is AI replacing software developers, or is it changing who can participate in software development?', 'Quote'), Spacer(1, 8*mm), two_cards([('The opportunity', 'People with ideas and domain knowledge can build working software more quickly.'), ('The disruption', 'The time, knowledge, and team size traditionally required to create software are shrinking.')], fill=PALE), Spacer(1, 10*mm), P('This document is based on a practical experience rather than a prediction about the future.', 'Body'), PageBreak()]

# 3
story += [P('02 / MY STARTING POINT', 'Kicker'), P('I started outside my comfort zone', 'PageTitle'), P('I am primarily a backend developer. Before this experience, I had limited practical knowledge of React, JavaScript, frontend architecture, component-based development, client-side routing, and frontend API integration.', 'Body'), P('I did not wait until I had mastered frontend development before beginning. I used AI as a development partner and learned while building.', 'Body'), Spacer(1, 6*mm), two_cards([('What I already understood', 'Backend APIs, business rules, authentication requirements, database behavior, and the product I wanted to build.'), ('What I needed to learn', 'How the frontend was structured, how components communicated, how requests moved, and how the interface connected to the backend.')], fill=SKY), Spacer(1, 12*mm), P('The old sequence was often: learn first, build later. My sequence became: start building, then learn in context.', 'QuoteSmall'), PageBreak()]

# 4
story += [P('03 / WHAT I BUILT', 'Kicker'), P('From backend requirements to a working frontend', 'PageTitle'), P('With AI guidance, I developed a functional frontend for an Academy Management System.', 'Body'), two_cards([('Authentication', 'Login, tokens, protected requests, session restoration, password change, and password reset.'), ('Application shell', 'Dashboard navigation, responsive layout, sidebar, top bar, professional forms, and feedback states.')], fill=WHITE), Spacer(1, 6*mm), two_cards([('Management features', 'Academies, users, role and permission management, endpoint-level access, and academy-scoped data.'), ('Integration', 'Frontend requests connected to existing backend APIs with success, validation, unauthorized, forbidden, and error messages.')], fill=WHITE), Spacer(1, 12*mm), P('A person with limited frontend experience can now move from an existing backend system to a usable interface with AI assistance.', 'QuoteSmall'), PageBreak()]

# 5
story += [P('04 / THE NEW WORKFLOW', 'Kicker'), P('AI changed how I moved forward', 'PageTitle'), P('The process became more direct and iterative:', 'Body')]
steps = [('1', 'Explain', 'Describe the requirement, API behavior, and desired result.'), ('2', 'Generate', 'Create a project structure, component, page, or integration.'), ('3', 'Run', 'Open the application and observe what actually happens.'), ('4', 'Share', 'Provide the error, confusing behavior, or missing requirement.'), ('5', 'Improve', 'Apply the next fix, test it, and continue building.')]
for n, title, body in steps:
    story.append(KeepTogether([Table([[P(n, 'Quote') , P(f'<b>{title}</b><br/>{body}', 'BodySmall')]], colWidths=[18*mm, 145*mm], style=TableStyle([('BACKGROUND',(0,0),(0,0),SKY),('VALIGN',(0,0),(-1,-1),'MIDDLE'),('LEFTPADDING',(0,0),(-1,-1),10),('RIGHTPADDING',(0,0),(-1,-1),10),('TOPPADDING',(0,0),(-1,-1),8),('BOTTOMPADDING',(0,0),(-1,-1),8),('BOX',(0,0),(-1,-1),0.5,LINE)])), Spacer(1, 4*mm)]))
story += [Spacer(1, 7*mm), P('AI did not require me to discover every solution alone. It helped me move from one development step to the next.', 'QuoteSmall'), PageBreak()]

# 6
story += [P('05 / THE BARRIER IS LOWER', 'Kicker'), P('Start before mastering every technology', 'PageTitle'), P('My experience showed me that AI can dramatically reduce the barrier to software development.', 'Body'), two_cards([('Start earlier', 'Begin with a clear idea instead of waiting until every technology is familiar.'), ('Learn in context', 'Understand files, components, requests, and errors while working on a real product.')], fill=SKY), Spacer(1, 6*mm), two_cards([('Build more', 'Generate components, API calls, authentication flows, navigation, and testing guidance.'), ('Move faster', 'Reduce the time spent searching for basic direction and common debugging steps.')], fill=SKY), Spacer(1, 13*mm), P('A person with domain knowledge, a clear idea, persistence, and access to AI can now build much more than before.', 'Quote'), PageBreak()]

# 7
story += [P('06 / BEFORE FULL UNDERSTANDING', 'Kicker'), P('AI can provide a path forward', 'PageTitle'), P('One of the most important changes is that AI can sometimes provide a useful solution before the developer fully understands the problem.', 'Body'), P('It can explain:', 'Body'), two_cards([('Project flow', 'Which file is responsible, which component calls another, and how a request moves through the application.'), ('Next action', 'What to change, what to run, what to test, and what information is needed when an error appears.')], fill=PALE), Spacer(1, 8*mm), P('This changes the learning process. Instead of learning everything first and building later, people can begin building and learn in context.', 'QuoteSmall'), Spacer(1, 10*mm), P('That is not a small convenience. It changes who can enter the field and how quickly they can produce a first version.', 'Body'), PageBreak()]

# 8
story += [P('07 / THE RISK IS REAL', 'Kicker'), P('AI is changing the economics of development', 'PageTitle'), P('AI is not only making experienced developers faster. It is also allowing people from one technical area to work in another area more quickly.' , 'Body'), Spacer(1, 2*mm), two_cards([('Smaller teams', 'Companies may produce more software with fewer people.'), ('Entry-level pressure', 'Some basic implementation and repetitive tasks may be automated.')], fill=HexColor('#FFF4F4')), Spacer(1, 6*mm), two_cards([('Higher expectations', 'Clients may expect faster delivery at lower cost.'), ('More competition', 'A single person may perform work that previously required multiple specialists.')], fill=HexColor('#FFF4F4')), Spacer(1, 10*mm), P('The risk is not imaginary. AI is already changing the amount of time and knowledge required to produce software.', 'QuoteSmall'), PageBreak()]

# 9
story += [P('08 / WHAT AI DOES NOT GUARANTEE', 'Kicker'), P('Generated code is not automatically reliable', 'PageTitle'), P('AI can create software, but the output can still contain serious problems.', 'Body'), two_cards([('Technical risks', 'Security vulnerabilities, poor architecture, performance issues, incorrect API integration, and hidden maintenance costs.'), ('Product risks', 'Incorrect business assumptions, incomplete validation, poor data handling, and a solution that does not match the real user need.')], fill=PALE), Spacer(1, 10*mm), P('The difference is not that these risks disappear. The difference is that AI can now help with many activities that previously slowed development down.', 'Body'), P('Someone still needs to decide whether the solution is appropriate. Someone must take responsibility for the final product.', 'QuoteSmall'), PageBreak()]

# 10
story += [P('09 / THE ROLE IS CHANGING', 'Kicker'), P('The developer is becoming more than a code writer', 'PageTitle'), P('The future developer may spend less time typing repetitive code and more time making decisions that determine whether the product succeeds.', 'Body'), two_cards([('Human strengths', 'Business understanding, product thinking, communication, problem definition, and ownership.'), ('Technical strengths', 'System design, security awareness, testing, data protection, review, and maintenance.')], fill=SKY), Spacer(1, 10*mm), P('The question may no longer be:', 'Body'), P('“Can you write this code?”', 'Quote'), P('It may become:', 'Body'), P('“Can you use the available tools to create the right solution safely and reliably?”', 'Quote'), PageBreak()]

# 11
story += [P('10 / MY PERSONAL LESSON', 'Kicker'), P('I was able to start before I had mastered everything', 'PageTitle'), P('This experience changed my view of software development.', 'Body'), P('AI allowed me to build in an area where I had very little previous knowledge. It helped me understand project structure, follow the request flow, connect frontend and backend systems, solve errors, and continue moving forward.', 'Body'), Spacer(1, 6*mm), P('I did not become an expert frontend developer overnight.', 'Quote'), P('But I was able to start before I had mastered everything.', 'Quote'), Spacer(1, 9*mm), P('That is the major change: AI made it possible to learn and build at the same time.', 'Body'), Spacer(1, 8*mm), two_cards([('The opportunity', 'More people can turn ideas into prototypes and products.'), ('The responsibility', 'People must still understand the goal, review the result, and own the consequences.')], fill=PALE), PageBreak()]

# 12
story += [Spacer(1, 13*mm), P('THE BIGGER QUESTION', 'Kicker'), P('AI is changing who can build software.', 'PageTitle'), P('It is reducing the time, knowledge, and resources traditionally required to create software.', 'Body'), P('The threat is real for repetitive work and some entry-level tasks. The opportunity is real for people who learn to use AI effectively and take responsibility for what they build.', 'Body'), Spacer(1, 8*mm), P('The future may not belong only to people who write every line of code manually. It may belong to people who can understand a real problem, explain it clearly, use AI to accelerate development, review the result, protect the system, test the product, and deliver something useful.', 'QuoteSmall'), Spacer(1, 17*mm)]
closing = Table([[P('Is AI replacing software developers, or is it creating a new type of developer?', 'WhiteQuote')], [P('What has your practical experience been? Share your honest opinion, concerns, or a different view of the future.', 'WhiteBodySmall')]], colWidths=[165*mm])
closing.setStyle(TableStyle([('BACKGROUND',(0,0),(-1,-1),NAVY),('TEXTCOLOR',(0,0),(-1,-1),WHITE),('LEFTPADDING',(0,0),(-1,-1),18),('RIGHTPADDING',(0,0),(-1,-1),18),('TOPPADDING',(0,0),(-1,0),22),('BOTTOMPADDING',(0,0),(-1,0),10),('TOPPADDING',(0,1),(-1,1),0),('BOTTOMPADDING',(0,1),(-1,1),22)]))
story += [closing, Spacer(1, 14*mm), P('A practical reflection on AI-assisted software development', 'BodySmall')]

doc = StoryDoc(str(OUT), title='AI and the Future of Software Development', author='A backend developer learning while building')
doc.build(story)
print(OUT)

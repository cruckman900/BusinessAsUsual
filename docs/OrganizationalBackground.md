# Organizational Background - Corporate Campus Visualization

## Overview
A professional, animated SVG background that visualizes an organization as a busy corporate campus with realistic people walking on pathways, connected by mechanical infrastructure (gears) that represent the business machinery.

## Visual Concept
> **"A busy sidewalk of professionals, moving in groups through a corporate campus, all part of an interconnected machine."**

### Design Elements

**1. Realistic People** (39 total)
- Natural walking poses with proper proportions
- Three variations:
  - Regular walking (mid-stride)
  - Walking alternate stride
  - Business person with briefcase
- Organized into 8 department groups
- Natural scale (not weird stick figures!)

**2. Walkways**
- Horizontal paths connecting departments (like sidewalks)
- Vertical connectors between levels
- Subtle gradient and pulsing effect to show activity
- Creates a campus-like grid structure

**3. Mechanical Infrastructure**
- Large gear hubs at major department intersections
- Medium gears at connection points
- Small gears filling gaps
- Gears rotate at different speeds (slower = more realistic)
- Represents the machinery that connects everyone

**4. Department Labels**
- Clean, readable text above each group
- Shows organization structure:
  - HR (4 people)
  - Finance (5 people)
  - Operations (6 people)
  - IT (4 people)
  - Sales (5 people)
  - Marketing (4 people)
  - Engineering (7 people - largest team)
  - Customer Success (4 people)

## Animations

### People Walking
```css
- walk-motion: 4s - subtle horizontal sway simulating walking
- walk-motion-reverse: 4.5s - opposite direction walkers
- Staggered timing for natural, organic feel
- Drop shadows add depth
```

### Gears (Infrastructure)
```css
- Large gears: 40s rotation (slow, powerful)
- Medium gears: 30s rotation
- Small gears: 20s rotation
- Alternating clockwise/counter-clockwise for interconnection
```

### Walkways
```css
- Subtle pulse: 8s - shows activity/foot traffic
- Opacity shifts between 30-40%
```

## Technical Features

- **📐 Fully Scalable**: SVG-based, perfect on any resolution
- **🚀 Performance**: Pure CSS animations, no JavaScript
- **📱 Responsive**: Reduced opacity on smaller screens
- **🎨 18% Opacity**: Visible but doesn't overpower content
- **🖱️ Non-interactive**: `pointer-events: none`
- **⚡ Hardware Accelerated**: Uses transforms for smooth 60fps

## Integration

```razor
@using BusinessAsUsual.Web.Components.Shared

<link href="css/organizational-background.css" rel="stylesheet" />

<div class="your-container">
	<OrganizationalBackground />
	<div class="your-content" style="position: relative; z-index: 1;">
		<!-- Your content here -->
	</div>
</div>
```

## Design Philosophy

### The Metaphor
- **Walkways** = Communication channels
- **People** = Individual contributors
- **Gears** = Business processes and systems
- **Movement** = Constant productivity
- **Interconnection** = Departments working together

### Professional Aesthetic
- Natural human proportions (not cartoon-like)
- Realistic walking poses
- Corporate campus vibe
- Clean, modern styling
- Subtle, not distracting

## Files
```
frontend/BusinessAsUsual.Web/
├── Components/Shared/
│   └── OrganizationalBackground.razor       # SVG component
└── wwwroot/css/
	└── organizational-background.css        # Animation styles
```

## Customization

### Adjust Opacity
```css
.organizational-background {
	opacity: 0.18; /* Change to taste: 0.1-0.3 recommended */
}
```

### Animation Speed
```css
.person-walking {
	animation: walk-motion 4s; /* Slower = more realistic */
}

.gear-large {
	animation: rotate-clockwise 40s; /* Slower = more powerful feel */
}
```

### Add More People
Simply duplicate a person group in the SVG:
```razor
<g class="person-walking" transform="translate(X, Y)">
	<use href="#person-walking"/>
</g>
```

## Result
A sophisticated, professional background that:
- ✅ Shows realistic people, not weird silhouettes
- ✅ Simulates a busy corporate campus
- ✅ Uses gears as infrastructure/machinery
- ✅ Creates depth with walkways and levels
- ✅ Maintains visual interest without distraction
- ✅ Scales beautifully on all devices

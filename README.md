# LearnCarbon

Empower architects to analyze the relationship between structure & embodied carbon in early design stages!

## Overview

Looking at the projected construction growth between now and 2040, it's clear that embodied carbon plays a crucial role in sustainability. The earlier we can analyze and optimize for embodied carbon, the greater the impact on reducing environmental costs and project expenses.

**LearnCarbon** is a Rhino plugin that integrates with a machine learning backend to provide architects and designers with tools to predict carbon footprints in the early stages of design.

### Features:
- **Model A**: With a single click, input a conceptual massing model and gather data on area, total built-up space, and structure type. It predicts the Global Warming Potential (GWP).
- **Model B**: Input area, total built-up space, and target GWP value to predict the most suitable structural type to meet sustainability goals.

For more information, visit [LearnCarbon](https://learncarbon.me/).

---

## Setup Instructions

This version of LearnCarbon has been updated for **Rhino 8** and no longer requires Hops through Flask. The plugin and machine learning backend can now be run directly via Rhino's script editor.

### Prerequisites
Ensure you have Rhino 8 installed and Python set up for running scripts.

### Step-by-Step Guide:

### 1. Clone the Repositories
You need to clone both the LearnCarbon Rhino Plugin and the [Machine Learning backend](https://github.com/LearnCarbon/src/tree/main) to the same parent directory.

### 2. Install Required Libraries
To ensure the LearnCarbon Rhino Plugin functions correctly, you'll need to install the necessary Python libraries for the machine learning models.

- Open Rhino’s script editor (`ScriptEditor`).
- In the script editor, navigate to the folder where you cloned the `src` repository.
- Run the `run_ML.py` script from the `src` repository. This script will automatically install all the required dependencies. You only need to perform this step once at the beginning.

### 3. Build and Install the Plugin
After installing the required libraries, you’ll need to build and install the Rhino plugin:

- Open the **Tool_LearnCarbonRhinoPlugin** project in your preferred development environment.
- Follow the Rhino plugin development guidelines to build the project.
- Once built, install the plugin within Rhino:
  1. Open Rhino and navigate to `Tools` -> `Options` -> `Plugins`.
  2. Click `Install` and select the built plugin from your local directory.
  3. Enable the LearnCarbon plugin.

Now, you should be ready to start using LearnCarbon in Rhino 8 to make early-stage design decisions informed by embodied carbon analysis.

---

## Contributing

We welcome contributions to LearnCarbon! Please feel free to open issues or submit pull requests to improve the plugin or the machine learning backend. Here are some ways you can contribute:
- Reporting bugs or issues
- Suggesting new features
- Contributing code improvements
- Enhancing documentation




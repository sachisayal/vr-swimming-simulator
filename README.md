# VR Swimming Simulator for Aquatic Skill Learning

An immersive virtual reality system designed to support swimming skill development through real-time motion tracking, biomechanical analysis, and interactive stroke visualization.

This project was developed as part of my **Barrett Honors Thesis at Arizona State University** and explores how virtual reality can be used to analyze swimming technique and create more accessible, technology-driven approaches to aquatic skill learning.

## Overview

The VR Swimming Simulator is a real-time Unity application developed in **C#** that tracks and evaluates swimming movements in a virtual environment.

The system uses **six-degrees-of-freedom (6DoF) tracking**, **inverse kinematics**, and **ellipse-based motion modeling** to represent upper-body movement and evaluate multiple biomechanical characteristics of a swimmer's stroke.

The project combines virtual reality development, motion analysis, biomechanics, and real-time software systems to translate physical movement into measurable performance information.

## Key Features

* Real-time virtual reality swimming simulation
* 6DoF motion tracking
* Inverse kinematics for realistic arm and upper-body movement
* Evaluation of **4+ biomechanical stroke metrics per arm**
* Ellipse-based modeling of swimming stroke trajectories
* Modular C# architecture for real-time movement analysis
* Interactive visualization of swimming motion and technique
* Support for analyzing multiple components of swimming mechanics

## Technologies

* **Unity**
* **C#**
* **Virtual Reality**
* **6DoF Motion Tracking**
* **Inverse Kinematics**
* **Biomechanical Motion Analysis**
* **Real-Time Systems**
* **Data Analysis**

## System Design

The simulator processes tracked VR controller and body movement to reconstruct swimming motions inside the virtual environment.

Motion data is used to evaluate stroke characteristics and generate measurable biomechanical information. Inverse kinematics helps model realistic joint and arm movement, while ellipse-based motion modeling represents the curved trajectories commonly observed during swimming strokes.

The system was designed using a modular structure so that individual movement-analysis components can be developed and evaluated independently while operating together in real time.

## Project Structure

```text
vr-swimming-simulator/
│
├── Assets/
│   ├── Scripts/
│   ├── Scenes/
│   ├── Materials/
│   ├── Prefabs/
│   └── Other Unity Assets
│
├── Packages/
│
├── ProjectSettings/
│
├── README.md
│
└── .gitignore
```

## Running the Project

1. Clone the repository:

```bash
git clone https://github.com/sachisayal/vr-swimming-simulator.git
```

2. Open **Unity Hub**.

3. Select **Add project from disk**.

4. Choose the cloned `vr-swimming-simulator` directory.

5. Open the project using a compatible Unity version.

6. Connect the required VR hardware and launch the appropriate Unity scene.

## Project Goals

The project explores how immersive technology can provide an alternative method for learning and analyzing swimming technique.

Traditional swimming instruction depends heavily on in-person observation and access to aquatic facilities. A virtual environment creates opportunities to study movement outside of the pool while providing measurable information about stroke mechanics that may be difficult to identify through visual observation alone.

The broader goal is to investigate how **VR, biomechanical modeling, and real-time feedback systems** can contribute to safer and more accessible aquatic skill development.

## Skills Demonstrated

This project provided hands-on experience with:

* Virtual reality application development
* C# software development
* Unity development
* Inverse kinematics
* Motion tracking
* Biomechanical analysis
* Real-time data processing
* Modular system design
* Human-computer interaction
* Technical research and experimentation

## Honors Thesis

**Enhancing Aquatic Skill Learning Through a Virtual Reality Swimming Simulator**

Barrett, The Honors College
Arizona State University

The research investigated the application of immersive virtual reality and biomechanical motion analysis to aquatic skill learning.

## Future Development

Potential extensions include:

* Expanded biomechanical performance metrics
* More detailed real-time feedback
* Additional stroke-analysis functionality
* Improved visualization of movement trajectories
* Personalized performance comparisons
* Expanded support for different VR hardware configurations

## Author

**Sachi Sayal**

B.S. Data Science — Computer Science Concentration
Arizona State University | Barrett, The Honors College

## License

This repository contains work developed for an academic honors thesis. Please contact the author before reproducing or redistributing substantial portions of the project.

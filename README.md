# Cumulus Cloud Animation by Continuous Cellular Automata

This project proposes a unique method of cumulus cloud animation using continuous cellular automata on a 3D scalar field that represents clouds with a focus on realistic formation and dissipation.

## Description

Clouds are represented as a 3-dimensional scalar field in which each cell has a humidity between 0 and 1. The scalar field wraps at the edges to avoid cloud death at the edges and corners.
Clouds are rendered in a simplistic way since the goal of this project was the animation. Rendering is done by giving each cell a cube with a edge length that corresponds to it's humidity value.

The algorithm utilizes a continuous cellular automata ruleset and random noise to promote unique cloud generation. The ruleset uses 1 accumulating, 2 decaying, and one clamping rule. Each change in humidity is multiplied by it's corresponding weight before being applied to the cell.

The accumulation rule utilizes a global wind vector that promotes clouds building in the direction of the wind. It does this by taking all adjacent cells to the current cell and accumulating a percentage of the humidity corresponding to the displacement vector's similarity to the global wind vector.

The random decay rule decays a cell by a random amount between the humidity of the cell and zero. This helps promote interesting formation and avoid an "ideal" large mass shape that sustains itself.

The porportional decay rule calculates the mean humidity across all cloud cells and then negates it from all cells equally. This rule helps keep an average humidity to keep clouds from collapsing or forming unintentionally.

The clamping rule simply clamps each cell's humidity to be between the minimum humidity (represented by the global humidity variable) and 1. Having a minimum humidity ensures the accumulation rule always has some humidity to accumulate from.

Adjusting the sum weight during runtime allows us to force cloud formation and dissipation by increasing it or decreasing it respectively. Doing so yields these following results:

![anim_accumulating_video-ezgif com-crop (1)](https://github.com/user-attachments/assets/6a51b305-8e40-4c42-aab9-8f9369a46de4)

Figure 1: Nimbus Accumulation

![anim_decaying_video-ezgif com-crop](https://github.com/user-attachments/assets/0622192b-20f4-40d0-97a5-76fd525fe5f3)

Figure 2: Nimbus Dissipation

For more in-depth information of the logistics, an academic report was created on this project and is in the repository titled `CSC_473_Nimbus_Animation.pdf`.

## Getting Started

### Dependencies

* Unity: Version 2023.1.0b11

### Installing

* Clone repository with `git clone https://github.com/williamt1117/CloudAnimation`
* Open repository with Unity

### Executing program

* Select 'Particle System' under the hierarchy
* Adjust variables under 'Cloud Simulator' if wanted
* Press the play button at the top and view visualization through the game window or scene window

### Variables

* Size X, Y, and Z : cloud bounding volume size
* Humidity : minimum cell value
* Sum variables
  * Wind Vector X, Y, and Z : direction of wind
  * minimum wind contribution : minimum percentage of cell's value contributed
* Rule weight variables
  * sum weight : multiplier for adjacency sum rule
  * random decay weight : multiplier for random decay rule
  * fixed porportional decay weight : multiplier for fixed porportional decay rule
* (deprecated) Perlin Decay variables
  * perlin frequency : frequency of perlin noise
  * perlin offset : offset of perlin noise
  * perlin decay weight : multiplier for perlin decay rule
* (deprecated) Overcrowding variables
  * overcrowded percentage : minimum humidity level to activate overcrowding rule
  * overcrowded decay weight : multiplier for overcrowding decay rule

## Authors

William Trottier [(GitHub)](https://github.com/williamt1117) [(LinkedIn)](https://www.linkedin.com/in/william-trottier/)

## Future Work

* Optimize for run-time to make usable real-time
* Change wind vector to vector field
* Add rule to promote existence of stratus-like clouds

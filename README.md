# Quadrat Count

Compare individual plots with pooled diversity and species occurrence.

## Try it

From this folder with the .NET 10 SDK installed:

```sh
dotnet run -- plots study.tsv
```

Each row gives plot ID, species and count. Use a dash and zero to record an empty plot. Simpson diversity is 1 minus the sum of squared proportions. An empty plot has undefined diversity.

The included study.tsv uses tab-separated fields. Comment lines start with `#`.

The original exercise remains available with `dotnet run -- example.tsv`.

## When to use it

An ecology class compares species composition between fixed sampling plots.

Example result:

```text
Q1: individuals 20, richness 1, Simpson 0
Q2: individuals 20, richness 1, Simpson 0
Q3: individuals 0, richness 0, Simpson undefined
Pooled: individuals 40, richness 2, Simpson 0.5
Daisy: occupied 1/3 plots
Grass: occupied 1/3 plots
```

Related course or field workflow: [Open University quantitative ecology practical](https://www.open.edu/openlearn/science-maths-technology/geology/practising-science-reading-the-rocks-and-ecology/content-section-2.4).

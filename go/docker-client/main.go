package main

import (
	"context"
	"flag"
	"fmt"

	"github.com/docker/docker/api/types"
	"github.com/docker/docker/client"
)

var (
	host string
)

func init() {
	flag.StringVar(&host, "host", client.DefaultDockerHost, "host to reach")
}

func main() {
	flag.Parse()
	ctx := context.Background()

	cli, err := client.NewClient(host, "", nil, nil)
	if err != nil {
		panic(err)
	}

	df, err := cli.DiskUsage(ctx)
	if err != nil {
		fmt.Printf("Cannot get disk usage\n")
	} else {
		fmt.Printf("Disk usage: %d\n", df.LayersSize)
	}

	containers, err := cli.ContainerList(ctx, types.ContainerListOptions{All: true})
	if err != nil {
		panic(err)
	}

	for _, container := range containers {
		fmt.Printf("%s %s\n", container.ID[:10], container.Image)
	}
}
